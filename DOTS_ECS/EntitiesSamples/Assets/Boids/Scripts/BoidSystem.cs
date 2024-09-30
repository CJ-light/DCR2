using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Mike's GDC Talk on 'A Data Oriented Approach to Using Component Systems'
// is a great reference for dissecting the Boids sample code:
// https://youtu.be/p65Yt20pw0g?t=1446
// It explains a slightly older implementation of this sample but almost all the
// information is still relevant.

// The targets (2 red fish) and obstacle (1 shark) move based on the ActorAnimation tab
// in the Unity UI, so that they are moving based on key-framed animation.

namespace Boids
{
    //RequireMatchingQueriesForUpdate: Use RequireMatchingQueriesForUpdate to force a System to skip calling OnUpdate if every EntityQuery in the system is empty.

    //Ordering of jobs by groups, groups are ran at a specific order so this is a way to order jobs with groups
    //UpdateInGroup: The jobs defined here are ran at this group
    //  SimulationSystemGroup is one of unity's premade system groups, its run at the update phase
    //UpdateBefore: Order systems relative to other systems. The attribute specified here has to be a member of the same group. In this example, this means that these jobs need to be done before doing the jobs in the TransformSystemGroup
    //  TransformSystemGroup: This is also one of unity's premade groups, is part of SimulationSystemGroup.

    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BoidSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Boid, BoidTarget, BoidObstacle: These are entities baked in the Authoring files
            //LocalToWorld: This is a 4x4 matrix that lets you translate loacl coordinates to that world's coordinates
            //SystemAPIQueryBuilder():
                //WithAll<>(): Specifices all the read-only component types that must be present
                //WithAll<>(): Specifies all the read and write component types that must be present
                //Build(): Create the query and put it in a variable, to be used for later

            //boidQuery :: Define a query to get all entities that have Boid and LocalToWorld Components
            //targetQuery :: Define a query to get all entities that have BoidTarget and LocalToWorld components
            //obstacleQuery :: Define a query to get all entities that have BoidObstacle and LocalToWorld components

            var boidQuery = SystemAPI.QueryBuilder().WithAll<Boid>().WithAllRW<LocalToWorld>().Build();
            var targetQuery = SystemAPI.QueryBuilder().WithAll<BoidTarget, LocalToWorld>().Build();
            var obstacleQuery = SystemAPI.QueryBuilder().WithAll<BoidObstacle, LocalToWorld>().Build();

            //Count the amount of entities that fit that query
            var obstacleCount = obstacleQuery.CalculateEntityCount();
            var targetCount = targetQuery.CalculateEntityCount();

            //world :: Access to the world, this is the place that has entities, systems and data
            //state.EntityManager.GetAllUniqueSharedComponents... :: Returns a list of unique entities of type <Boid> and puts it in uniqueBoidTypes
            //  note: The first entry of the array is the default value for the Boid type
            //  In a way, this is kind of looking for each fish type's configuration
            var world = state.WorldUnmanaged;
            state.EntityManager.GetAllUniqueSharedComponents(out NativeList<Boid> uniqueBoidTypes, world.UpdateAllocator.ToAllocator);
            float dt = math.min(0.05f, SystemAPI.Time.DeltaTime);

            // Each variant of the Boid represents a different value of the SharedComponentData and is self-contained,
            // meaning Boids of the same variant only interact with one another. Thus, this loop processes each
            // variant type individually.
            foreach (var boidSettings in uniqueBoidTypes)
            {
                // From the past boidQuery of all boids, filter by the shared components of each type of fish (each different school)
                boidQuery.AddSharedComponentFilter(boidSettings);

                var boidCount = boidQuery.CalculateEntityCount();
                if (boidCount == 0)
                {
                    // Early out. If the given variant includes no Boids, move on to the next loop.
                    // For example, variant 0 will always exit early bc it's it represents a default, uninitialized
                    // Boid struct, which does not appear in this sample.
                    boidQuery.ResetFilter();
                    continue;
                }

                // The following calculates spatial cells of neighboring Boids
                // note: working with a sparse grid and not a dense bounded grid so there
                // are no predefined borders of the space.

                //hashMap :: int keys, int values, capacity is boidCount, Allocator rewinds each world update
                //  Each key can have multiple values
                
                var hashMap                   = new NativeParallelMultiHashMap<int, int>(boidCount, world.UpdateAllocator.ToAllocator);
                var cellIndices               = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
                var cellObstaclePositionIndex = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
                var cellTargetPositionIndex   = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
                var cellCount                 = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
                var cellObstacleDistance      = CollectionHelper.CreateNativeArray<float, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
                var cellAlignment             = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
                var cellSeparation            = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(boidCount, ref world.UpdateAllocator);

                var copyTargetPositions       = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(targetCount, ref world.UpdateAllocator);
                var copyObstaclePositions     = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(obstacleCount, ref world.UpdateAllocator);

                // The following jobs all run in parallel because the same JobHandle is passed for their
                // input dependencies when the jobs are scheduled; thus, they can run in any order (or concurrently).
                // The concurrency is property of how they're scheduled, not of the job structs themselves.
                // CalculateBaseEntityIndexArrayAsync :: generates an array containing the index of the first entity within each chunk, relative to the list of entities that match this query.

                //These three return two things:
                // ...EntityIndexArray :: This is an array that contains the index of the first entity within each chunk
                // ...IndexJobHandle :: This is used to store defined jobs, to then be able to make dependencies and schedule jobs
                var boidChunkBaseEntityIndexArray = boidQuery.CalculateBaseEntityIndexArrayAsync(
                    world.UpdateAllocator.ToAllocator, state.Dependency,
                    out var boidChunkBaseIndexJobHandle);
                var targetChunkBaseEntityIndexArray = targetQuery.CalculateBaseEntityIndexArrayAsync(
                    world.UpdateAllocator.ToAllocator, state.Dependency,
                    out var targetChunkBaseIndexJobHandle);
                var obstacleChunkBaseEntityIndexArray = obstacleQuery.CalculateBaseEntityIndexArrayAsync(
                    world.UpdateAllocator.ToAllocator, state.Dependency,
                    out var obstacleChunkBaseIndexJobHandle);

                // These jobs extract the relevant position, heading component
                // to NativeArrays so that they can be randomly accessed by the `MergeCells` and `Steer` jobs.
                // These jobs are defined using the IJobEntity syntax.
                
                // Instantiate the initialBoidJob job and store it into its handle
                var initialBoidJob = new InitialPerBoidJob
                {
                    ChunkBaseEntityIndices = boidChunkBaseEntityIndexArray,
                    CellAlignment = cellAlignment,
                    CellSeparation = cellSeparation,
                    ParallelHashMap = hashMap.AsParallelWriter(),
                    InverseBoidCellRadius = 1.0f / boidSettings.CellRadius,
                };

                var initialBoidJobHandle = initialBoidJob.ScheduleParallel(boidQuery, boidChunkBaseIndexJobHandle);

                // Instantiate the initialTargetJob and store it into its handle
                var initialTargetJob = new InitialPerTargetJob
                {
                    ChunkBaseEntityIndices = targetChunkBaseEntityIndexArray,
                    TargetPositions = copyTargetPositions,
                };
                var initialTargetJobHandle = initialTargetJob.ScheduleParallel(targetQuery, targetChunkBaseIndexJobHandle);

                // Instantiate the initialObstacleJob and store it into its handle
                var initialObstacleJob = new InitialPerObstacleJob
                {
                    ChunkBaseEntityIndices = obstacleChunkBaseEntityIndexArray,
                    ObstaclePositions = copyObstaclePositions,
                };
                var initialObstacleJobHandle = initialObstacleJob.ScheduleParallel(obstacleQuery, obstacleChunkBaseIndexJobHandle);

                // MemsetNativeArray :: copy value repeditley into source
                //  Turn all values of source into the value 
                //  Turn all values in cellCount into 1
                var initialCellCountJob = new MemsetNativeArray<int>
                {
                    Source = cellCount,
                    Value  = 1
                };
                var initialCellCountJobHandle = initialCellCountJob.Schedule(boidCount, 64, state.Dependency);

                //CombineDependencies(Job1, Job2) :: Combine the dependencies of both jobs, so that the jobs that depend on these need to wait for both of these before going next
                var initialCellBarrierJobHandle = JobHandle.CombineDependencies(initialBoidJobHandle, initialCellCountJobHandle);
                var copyTargetObstacleBarrierJobHandle = JobHandle.CombineDependencies(initialTargetJobHandle, initialObstacleJobHandle);
                var mergeCellsBarrierJobHandle = JobHandle.CombineDependencies(initialCellBarrierJobHandle, copyTargetObstacleBarrierJobHandle);


                var mergeCellsJob = new MergeCells
                {
                    cellIndices               = cellIndices,
                    cellAlignment             = cellAlignment,
                    cellSeparation            = cellSeparation,
                    cellObstacleDistance      = cellObstacleDistance,
                    cellObstaclePositionIndex = cellObstaclePositionIndex,
                    cellTargetPositionIndex   = cellTargetPositionIndex,
                    cellCount                 = cellCount,
                    targetPositions           = copyTargetPositions,
                    obstaclePositions         = copyObstaclePositions
                };
                var mergeCellsJobHandle = mergeCellsJob.Schedule(hashMap, 64, mergeCellsBarrierJobHandle);

                // This reads the previously calculated boid information for all the boids of each cell to update
                // the `localToWorld` of each of the boids based on their newly calculated headings using
                // the standard boid flocking algorithm.
                var steerBoidJob = new SteerBoidJob
                {
                    ChunkBaseEntityIndices = boidChunkBaseEntityIndexArray,
                    CellIndices = cellIndices,
                    CellCount = cellCount,
                    CellAlignment = cellAlignment,
                    CellSeparation = cellSeparation,
                    CellObstacleDistance = cellObstacleDistance,
                    CellObstaclePositionIndex = cellObstaclePositionIndex,
                    CellTargetPositionIndex = cellTargetPositionIndex,
                    ObstaclePositions = copyObstaclePositions,
                    TargetPositions = copyTargetPositions,
                    CurrentBoidVariant = boidSettings,
                    DeltaTime = dt,
                    MoveDistance = boidSettings.MoveSpeed * dt,
                };
                var steerBoidJobHandle = steerBoidJob.ScheduleParallel(boidQuery, mergeCellsJobHandle);

                // Dispose allocated containers with dispose jobs.
                state.Dependency = steerBoidJobHandle;

                // We pass the job handle and add the dependency so that we keep the proper ordering between the jobs
                // as the looping iterates. For our purposes of execution, this ordering isn't necessary; however, without
                // the add dependency call here, the safety system will throw an error, because we're accessing multiple
                // pieces of boid data and it would think there could possibly be a race condition.

                boidQuery.AddDependency(state.Dependency);
                boidQuery.ResetFilter();
            }

            //Delete what you put into this vairable
            uniqueBoidTypes.Dispose();
        }

                // In this sample there are 3 total unique boid variants, one for each unique value of the
        // Boid SharedComponent (note: this includes the default uninitialized value at
        // index 0, which isnt actually used in the sample).

        // This accumulates the `positions` (separations) and `headings` (alignments) of all the boids in each cell to:
        // 1) count the number of boids in each cell
        // 2) find the nearest obstacle and target to each boid cell
        // 3) track which array entry contains the accumulated values for each boid's cell
        // In this context, the cell represents the hashed bucket of boids that are near one another within cellRadius
        // floored to the nearest int3.
        // Note: `IJobNativeParallelMultiHashMapMergedSharedKeyIndices` is a custom job to iterate safely/efficiently over the
        // NativeContainer used in this sample (`NativeParallelMultiHashMap`). Currently these kinds of changes or additions of
        // custom jobs generally require access to data/fields that aren't available through the `public` API of the
        // containers. This is why the custom job type `IJobNativeParallelMultiHashMapMergedSharedKeyIndicies` is declared in
        // the DOTS package (which can see the `internal` container fields) and not in the Boids sample.
        [BurstCompile]
        struct MergeCells : IJobNativeParallelMultiHashMapMergedSharedKeyIndices
        {
            public NativeArray<int>                 cellIndices;
            public NativeArray<float3>              cellAlignment;
            public NativeArray<float3>              cellSeparation;
            public NativeArray<int>                 cellObstaclePositionIndex;
            public NativeArray<float>               cellObstacleDistance;
            public NativeArray<int>                 cellTargetPositionIndex;
            public NativeArray<int>                 cellCount;
            [ReadOnly] public NativeArray<float3>   targetPositions;
            [ReadOnly] public NativeArray<float3>   obstaclePositions;

            // NearestPosition(targets[],position) :: Compare the positions of the targets with the position of the object we want to compare to
            //  For all the targets, find the closest one to the positions
            //  Return:
            //      nearestPositionIndex :: index of the closest target
            //      nearestDistance :: distance of the closest target
            void NearestPosition(NativeArray<float3> targets, float3 position, out int nearestPositionIndex, out float nearestDistance)
            {
                nearestPositionIndex = 0;
                nearestDistance      = math.lengthsq(position - targets[0]);
                for (int i = 1; i < targets.Length; i++)
                {
                    var targetPosition = targets[i];
                    var distance       = math.lengthsq(position - targetPosition);
                    var nearest        = distance < nearestDistance;

                    nearestDistance      = math.select(nearestDistance, distance, nearest);
                    nearestPositionIndex = math.select(nearestPositionIndex, i, nearest);
                }
                nearestDistance = math.sqrt(nearestDistance);
            }

            // ExecuteFirst() :: This is a function inside the IJobNativeParallelMultiHashMapMergedSharedKeyIndices', its run for the first value in each unique key
            //  Executes on the first occurence of a hash in each bucket
            // Resolves the distance of the nearest obstacle and target and stores the cell index.
            public void ExecuteFirst(int index)
            {
                //position :: current position of that boid
                //  cellSeparation :: array with cell positions of each boid
                //  cellCount :: number of boids in that cell (I think in execute First this should always be 1(the default value))
                var position = cellSeparation[index] / cellCount[index];

                //Find the closest obstacle and distance to obstacle
                int obstaclePositionIndex;
                float obstacleDistance;
                NearestPosition(obstaclePositions, position, out obstaclePositionIndex, out obstacleDistance);
                cellObstaclePositionIndex[index] = obstaclePositionIndex;
                cellObstacleDistance[index]      = obstacleDistance;

                //Find the closest target and distance to it
                int targetPositionIndex;
                float targetDistance;
                NearestPosition(targetPositions, position, out targetPositionIndex, out targetDistance);
                cellTargetPositionIndex[index] = targetPositionIndex;

                //Save in the cellIndices
                cellIndices[index] = index;
            }

            // ExecuteNext() :: This is a function inside the IJobNativeParallelMultiHashMapMergedSharedKeyIndices', it runs for evey other value after the first occurence of a key
            //  It's run for every subsequent occurence of a hash in a bucket. 
            // Sums the alignment and separation of the actual index being considered and stores
            // the index of this first value where we're storing the cells.
            // note: these items are summed so that in `Steer` their average for the cell can be resolved.
            public void ExecuteNext(int cellIndex, int index)
            {
                //Add 1 to the cellCount, Double the cellAlignment and cellSeparation, save the boids into cellDindices
                cellCount[cellIndex]      += 1;
                cellAlignment[cellIndex]  += cellAlignment[cellIndex];
                cellSeparation[cellIndex] += cellSeparation[cellIndex];
                cellIndices[index]        = cellIndex;
            }
        }

        //InitialPerBoidJob :: Adds each boid into the Hashmap
        [BurstCompile]
        partial struct InitialPerBoidJob : IJobEntity
        {
            // NativeDisableParallelForRestriction :: apply this to a native array to disable some safety checks for IJobParallelFor and IJobParallelForBatch. These checks ensure that for any array your job is using, you are only writing to the current index (or an index within the batch range for the batched version). You can still read from them, even without using the attribute.
            //  tldr:: Use this attribute to let you write an index that is not the current one (which is something you usually can't do)
            //  I think this is safe to use in this case because its saving based on the position of that entity in memory, which will be unique,
            //      That means that there will not be any conflicts in overwriting the same place in memory.
            [ReadOnly] public NativeArray<int> ChunkBaseEntityIndices;
            [NativeDisableParallelForRestriction] public NativeArray<float3> CellAlignment;
            [NativeDisableParallelForRestriction] public NativeArray<float3> CellSeparation;
            public NativeParallelMultiHashMap<int, int>.ParallelWriter ParallelHashMap;
            public float InverseBoidCellRadius;

            //chunkIndexQuery :: What chunk you want to check
            //entityIndexInChunk ::  What specific value in chunk you want to check
            //in LocalToWorld localToWorld :: This is trying to access the LocalToWorld component of the entity that gets this job. 
                //'in' means that this is giong to be a read only value, if we wanted to be able to change it we would write 'ref LocalToWorld localToWorld' 
            void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk, in LocalToWorld localToWorld)
            {
                //entityIndexInQuery :: Finds the index in memory where that boid Entity is being stored
                //Look at the direction and position of that entity
                int entityIndexInQuery = ChunkBaseEntityIndices[chunkIndexInQuery] + entityIndexInChunk;
                CellAlignment[entityIndexInQuery] = localToWorld.Forward;
                CellSeparation[entityIndexInQuery] = localToWorld.Position;
                // Populates a hash map, where each bucket contains the indices of all Boids whose positions quantize
                // to the same value for a given cell radius so that the information can be randomly accessed by
                // the `MergeCells` and `Steer` jobs.
                // This is useful in terms of the algorithm because it limits the number of comparisons that will
                // actually occur between the different boids. Instead of for each boid, searching through all
                // boids for those within a certain radius, this limits those by the hash-to-bucket simplification.

                // hash :: Calculates the cell position hash of that boid inside the hashmap 
                //      Calculates the hash key
                //      InverseBoidCellRadius :: use the inverse to calculate the cell position
                // ParallelHashMap.Add() :: Adds the hash key and the value that is connected to it, respectively
                var hash = (int)math.hash(new int3(math.floor(localToWorld.Position * InverseBoidCellRadius)));
                ParallelHashMap.Add(hash, entityIndexInQuery);
            }
        }

        // The red fish are the targets
        //InitialPerTargetJob :: Save the postion of the targets based on the position of the target in memory
        [BurstCompile]
        partial struct InitialPerTargetJob : IJobEntity
        {
            [ReadOnly] public NativeArray<int> ChunkBaseEntityIndices;
            [NativeDisableParallelForRestriction] public NativeArray<float3> TargetPositions;
            void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk, in LocalToWorld localToWorld)
            {
                //entityIndexInQuery :: Entity position in memory of that target
                //TargetPositions :: save the position of that target
                int entityIndexInQuery = ChunkBaseEntityIndices[chunkIndexInQuery] + entityIndexInChunk;
                TargetPositions[entityIndexInQuery] = localToWorld.Position;
            }
        }

        // The Sharks are the obstacles
        // InitialPerObstacleJob :: Saves the positions of obstacles based on thir positions in memory
        [BurstCompile]
        partial struct InitialPerObstacleJob : IJobEntity
        {
            [ReadOnly] public NativeArray<int> ChunkBaseEntityIndices;
            [NativeDisableParallelForRestriction] public NativeArray<float3> ObstaclePositions;
            void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk, in LocalToWorld localToWorld)
            {
                //entityIndexInQuery :: Entity position in memory of that obstacle
                //ObstaclePositions :: save the position of that Obstacle
                int entityIndexInQuery = ChunkBaseEntityIndices[chunkIndexInQuery] + entityIndexInChunk;
                ObstaclePositions[entityIndexInQuery] = localToWorld.Position;
            }
        }

        // 
        [BurstCompile]
        partial struct SteerBoidJob : IJobEntity
        {
            //CurrentBoidVariant :: This is the type of fish or boid used (fish, shark, red fish)
            [ReadOnly] public NativeArray<int> ChunkBaseEntityIndices;
            [ReadOnly] public NativeArray<int> CellIndices;
            [ReadOnly] public NativeArray<int> CellCount;
            [ReadOnly] public NativeArray<float3> CellAlignment;
            [ReadOnly] public NativeArray<float3> CellSeparation;
            [ReadOnly] public NativeArray<float> CellObstacleDistance;
            [ReadOnly] public NativeArray<int> CellObstaclePositionIndex;
            [ReadOnly] public NativeArray<int> CellTargetPositionIndex;
            [ReadOnly] public NativeArray<float3> ObstaclePositions;
            [ReadOnly] public NativeArray<float3> TargetPositions;
            public Boid CurrentBoidVariant;
            public float DeltaTime;
            public float MoveDistance;
            void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk, ref LocalToWorld localToWorld)
            {
                int entityIndexInQuery = ChunkBaseEntityIndices[chunkIndexInQuery] + entityIndexInChunk;
                // temporarily storing the values for code readability
                var forward                           = localToWorld.Forward;
                var currentPosition                   = localToWorld.Position;
                var cellIndex                         = CellIndices[entityIndexInQuery];
                var neighborCount                     = CellCount[cellIndex];
                var alignment                         = CellAlignment[cellIndex];
                var separation                        = CellSeparation[cellIndex];
                var nearestObstacleDistance           = CellObstacleDistance[cellIndex];
                var nearestObstaclePositionIndex      = CellObstaclePositionIndex[cellIndex];
                var nearestTargetPositionIndex        = CellTargetPositionIndex[cellIndex];
                var nearestObstaclePosition           = ObstaclePositions[nearestObstaclePositionIndex];
                var nearestTargetPosition             = TargetPositions[nearestTargetPositionIndex];

                // Setting up the directions for the three main biocrowds influencing directions adjusted based
                // on the predefined weights:
                // 1) alignment - how much should it move in a direction similar to those around it?
                //  note: we use `alignment/neighborCount`, because we need the average alignment in this case; however
                //  alignment is currently the summation of all those of the boids within the cellIndex being considered.
                // normalizesafe() :: Does aditional checks to see that it doesn't divide by zero or doesnt normalize a very small number
                var alignmentResult     = CurrentBoidVariant.AlignmentWeight
                                          * math.normalizesafe((alignment / neighborCount) - forward);
                // 2) separation - how close is it to other boids and are there too many or too few for comfort?
                // note: here separation represents the summed possible center of the cell. We perform the multiplication
                // so that both `currentPosition` and `separation` are weighted to represent the cell as a whole and not
                // the current individual boid.
                var separationResult    = CurrentBoidVariant.SeparationWeight
                                          * math.normalizesafe((currentPosition * neighborCount) - separation);
                // 3) target - is it still towards its destination?
                var targetHeading       = CurrentBoidVariant.TargetWeight
                                          * math.normalizesafe(nearestTargetPosition - currentPosition);

                // creating the obstacle avoidant vector s.t. it's pointing towards the nearest obstacle
                // but at the specified 'ObstacleAversionDistance'. If this distance is greater than the
                // current distance to the obstacle, the direction becomes inverted. This simulates the
                // idea that if `currentPosition` is too close to an obstacle, the weight of this pushes
                // the current boid to escape in the fastest direction; however, if the obstacle isn't
                // too close, the weighting denotes that the boid doesnt need to escape but will move
                // slower if still moving in that direction (note: we end up not using this move-slower
                // case, because of `targetForward`'s decision to not use obstacle avoidance if an obstacle
                // isn't close enough).
                var obstacleSteering                  = currentPosition - nearestObstaclePosition;
                var avoidObstacleHeading              = (nearestObstaclePosition + math.normalizesafe(obstacleSteering)
                    * CurrentBoidVariant.ObstacleAversionDistance) - currentPosition;
                

                // the updated heading direction. If not needing to be avoidant (ie obstacle is not within
                // predefined radius) then go with the usual defined heading that uses the amalgamation of
                // the weighted alignment, separation, and target direction vectors.
                var nearestObstacleDistanceFromRadius = nearestObstacleDistance - CurrentBoidVariant.ObstacleAversionDistance;
                var normalHeading                     = math.normalizesafe(alignmentResult + separationResult + targetHeading);
                var targetForward                     = math.select(normalHeading, avoidObstacleHeading, nearestObstacleDistanceFromRadius < 0);

                // updates using the newly calculated heading direction
                var nextHeading                       = math.normalizesafe(forward + DeltaTime * (targetForward - forward));
                localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(
                        // TODO: precalc speed*dt
                        new float3(localToWorld.Position + (nextHeading * MoveDistance)),
                        quaternion.LookRotationSafe(nextHeading, math.up()),
                        new float3(1.0f, 1.0f, 1.0f))
                };
            }
        }
    }
}
