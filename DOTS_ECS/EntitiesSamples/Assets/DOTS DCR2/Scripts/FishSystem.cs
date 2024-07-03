using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;

//1. Replace Vector3 with float3
//  float3 is what is used for the LocalToWorld component, so if I were to use Vector3 then I'd have to constantly typecast it to Vector3 and back into LocalToWorld
//  float3 is better for fast ECS calculation
//  This means I cant use some of the things Vector3 has like magnitude or normalize
//  instead I use math.length() and math.normalize()
//
namespace DCR2
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]

    public partial struct FishSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Note:: I was having a problem where the GetAllUniqueComponents function was telling me that it didnt find any entities with SemiStaticSchool component and that I couoldnt fiter with multiple shared components, which no longer appeared when I added the SemiStaticSchool component in the EntityQuery, but I'm not sure why that happened...
            var fishQuery = SystemAPI.QueryBuilder().WithAll<SemiStaticSchool>().WithAll<Fish>().WithAll<DynamicSchool>().WithAllRW<LocalToWorld>().Build();
            var world = state.WorldUnmanaged;
            float dt = math.min(0.05f, SystemAPI.Time.DeltaTime);
            
            state.EntityManager.GetAllUniqueSharedComponents(out NativeList<SemiStaticSchool> uniqueFishComponents, world.UpdateAllocator.ToAllocator);
            foreach (var fishSettings in uniqueFishComponents)
            {
                fishQuery.AddSharedComponentFilter(fishSettings);

                var fishCount = fishQuery.CalculateEntityCount();
                //Default value in shared component filter
                if (fishCount == 0)
                {
                    fishQuery.ResetFilter();
                    continue;
                }

                var fishChunkBaseEntityIndexArray = fishQuery.CalculateBaseEntityIndexArrayAsync(
                world.UpdateAllocator.ToAllocator, state.Dependency,
                out var fishChunkBaseIndexJobHandle);
                
                //Get weights and alpha from the semi-static component
                float couzinDirectionWeight = fishSettings.couzinDirectionWeight;
                float centroidFollowingDirectionWeight = fishSettings.centroidFollowingDirectionWeight;
                int minCentroidDistance = fishSettings.minCentroidDistance;
                int maxCentroidDistance = fishSettings.maxCentroidDistance;
                float alpha = fishSettings.alpha;
                float rho = fishSettings.rho;


                //Create an array that contains each entity in this school
                NativeArray<Entity> fishEntities = fishQuery.ToEntityArray(Allocator.TempJob); 

                //Get a list of components of type LocalToWorld (positions and angle of entities), one can lookup a specific component by entity 
                //  ex: fishPositions[entity]
                //ComponentLookup<LocalToWorld> fishPositions = GetComponentLookup<LocalToWorld>(isReadOnly = true);

                //Create arrays where you're going to store the results from jobs
                var couzinDirections = CollectionHelper.CreateNativeArray<CouzinValues, RewindableAllocator>(fishCount, ref world.UpdateAllocator);
                var centroidFollowingDirections = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(fishCount, ref world.UpdateAllocator);
                float3 newCentroid = float3.zero;

                //Is it alright to name the variables same?
                var calculateCentroidJob = new CalculateCentroidJob
                {
                    newCentroid = newCentroid,
                };
                newCentroid = newCentroid/(float)fishCount;
                var calculateCentroidJobHandle = calculateCentroidJob.Schedule(fishQuery, fishChunkBaseIndexJobHandle);

                var getCentroidFollowingDirectionJob = new GetCentroidFollowingDirectionJob
                {
                    chunkBaseEntityIndices = fishChunkBaseEntityIndexArray,
                    centroidFollowingDirectionWeight = centroidFollowingDirectionWeight,
                    minCentroidDistance = minCentroidDistance,
                    maxCentroidDistance = maxCentroidDistance,
                    centroidFollowingDirections = centroidFollowingDirections,
                };
                var getCentroidFollowingDirectionJobHandle = getCentroidFollowingDirectionJob.ScheduleParallel(fishQuery, fishChunkBaseIndexJobHandle);

                var getCouzinDirectionJob = new GetCouzinDirectionJob
                {
                    chunkBaseEntityIndices = fishChunkBaseEntityIndexArray,
                    fishPositions = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                    fishEntities = fishEntities,
                    fishCount = fishCount,
                    couzinDirectionWeight = couzinDirectionWeight,
                    alpha = alpha,
                    rho = rho,
                    couzinDirections = couzinDirections,
                };
                var getCouzinDirectionJobHandle = getCouzinDirectionJob.ScheduleParallel(fishQuery, fishChunkBaseIndexJobHandle);

                var centroidsBarrierJobHandle = JobHandle.CombineDependencies(calculateCentroidJobHandle, getCentroidFollowingDirectionJobHandle);
                var centroidCouzinBarrierJobHandle = JobHandle.CombineDependencies(centroidsBarrierJobHandle,getCouzinDirectionJobHandle);
                
                
                var assignFinalDirectionJob = new AssignFinalDirectionJob
                {
                    chunkBaseEntityIndices = fishChunkBaseEntityIndexArray,
                    couzinDirectionWeight = couzinDirectionWeight,
                    centroidFollowingDirectionWeight = centroidFollowingDirectionWeight,
                    couzinDirections = couzinDirections,
                    centroidFollowingDirections = centroidFollowingDirections,
                    deltaTime = dt,
                };
                
                var assignFinalDirectionJobHandle = assignFinalDirectionJob.ScheduleParallel(fishQuery, centroidCouzinBarrierJobHandle);

                var assignCentroidJob = new AssignCentroidJob
                {
                    newCentroid = newCentroid,
                };
                var assignCentroidJobHandle = assignCentroidJob.ScheduleParallel(fishQuery, centroidsBarrierJobHandle);

                var assignCentroidFinalDirectionBarrierJobHandle = JobHandle.CombineDependencies(assignCentroidJobHandle, assignFinalDirectionJobHandle);

                state.Dependency = assignCentroidFinalDirectionBarrierJobHandle;

                fishQuery.AddDependency(state.Dependency);
                fishQuery.ResetFilter();

                //TODO :: define the calculate distnce from centroid job
                    //Calculate distance from centroid, update the goToCentroid variable if need be and calculate direction towards centroid, put it in the centroidFollowingDirection array
                //TODO :: define update Centroid Job
                    //Calculate the new centroid
                //TODO :: define the updateCentroid on each fish Job
                    //Update this new centroid on each fish and calcuate the new direction for them
            }
            uniqueFishComponents.Dispose();
        }
        //TODO :: define calcuate distance from centroid job
        //Calculate distance from centroid and update the goToCentroid variable if need be (Too close, just right or too far)
        [BurstCompile]
        partial struct GetCentroidFollowingDirectionJob : IJobEntity
        {
            [ReadOnly] public NativeArray<int> chunkBaseEntityIndices;
            [ReadOnly] public float centroidFollowingDirectionWeight;
            [ReadOnly] public int minCentroidDistance;
            [ReadOnly] public int maxCentroidDistance;

            [NativeDisableParallelForRestriction] public NativeArray<float3> centroidFollowingDirections;

            //in LocalToWorld localToWorld :: Access the LocalToWorld component as a read only value
            //ref Fish fish :: Access to the Fish component as read and write value
            void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk, in LocalToWorld localToWorld, in DynamicSchool dynamicSchool, ref Fish fish)
            {
                int entityIndexInQuery = chunkBaseEntityIndices[chunkIndexInQuery] + entityIndexInChunk;
                float3 position = localToWorld.Position;
                float3 centroid = dynamicSchool.centroid;
                bool goToCentroid = fish.goToCentroid; //Does this change the entity?
                float3 centroidFollowingDirection = float3.zero;
                
                if (centroidFollowingDirectionWeight > 0)
                {
                    if (math.length(centroid - position) > maxCentroidDistance)
                    {
                        goToCentroid = true;
                        centroidFollowingDirection = math.normalizesafe(centroid - position);
                    }
                    else if (goToCentroid && math.length(centroid - position) > minCentroidDistance)
                    {
                        centroidFollowingDirection = math.normalizesafe(centroid - position);
                    }
                    else
                    {
                        goToCentroid = false;
                    }
                }
                centroidFollowingDirections[entityIndexInQuery] = centroidFollowingDirection;
            }      
        }
        //TODO :: define seperation and allignment job
        //  Calculate the vectors for the seperation and allignment job
        [BurstCompile]
        partial struct GetCouzinDirectionJob : IJobEntity
        {
            [ReadOnly] public NativeArray<int> chunkBaseEntityIndices;
            [ReadOnly] public ComponentLookup<LocalToWorld> fishPositions;
            [ReadOnly] public NativeArray<Entity> fishEntities;
            [ReadOnly] public int fishCount;
            [ReadOnly] public float couzinDirectionWeight;
            [ReadOnly] public float alpha;
            [ReadOnly] public float rho;

            [NativeDisableParallelForRestriction] public NativeArray<CouzinValues> couzinDirections;
            void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk, in LocalToWorld localToWorld)
            {
                int curFishIndex = chunkBaseEntityIndices[chunkIndexInQuery] + entityIndexInChunk;
                float3 position = localToWorld.Position;
                float3 distance;
                float3 v = float3.zero;
                float3 seperation = float3.zero;
                float3 allignment = float3.zero;
                
                seperation = couzinDirections[curFishIndex].Seperation;
                allignment = couzinDirections[curFishIndex].Allignment;

                if (couzinDirectionWeight > 0)
                {
                    for (int i = 0; i < fishCount; i++)
                    {
                        if (i != curFishIndex)
                        {
                            distance = fishPositions[fishEntities[i]].Position - position;
                            if (math.length(distance) < rho)
                            {
                                seperation += distance/math.length(distance);  
                            }
                            else if (math.length(distance) < alpha)
                            {
                                allignment += distance/math.length(distance);
                                v += fishPositions[fishEntities[i]].Forward/math.length(fishPositions[fishEntities[i]].Forward);
                            }
                        }
                    }
                    seperation = -seperation;
                    allignment += v;

                    seperation /= math.length(seperation); 
                    allignment /= math.length(allignment);

                    couzinDirections[curFishIndex] = new CouzinValues
                    {
                        Seperation = seperation,
                        Allignment = allignment,
                    };
                }
            }
        }

        //TODO :: Define the Job to Calculate the new position of each fish
        // Calculate the final direction for each fish and add to the new centroid calculation base on this
        //TODO :: Maybe add delta time to calculating the new position
        [BurstCompile]
        partial struct AssignFinalDirectionJob : IJobEntity
        {
            [ReadOnly] public NativeArray<int> chunkBaseEntityIndices;
            [ReadOnly] public float couzinDirectionWeight;
            [ReadOnly] public float centroidFollowingDirectionWeight;
            
            [ReadOnly] public NativeArray<CouzinValues> couzinDirections;
            [ReadOnly] public NativeArray<float3> centroidFollowingDirections;

            [ReadOnly] public float deltaTime;

            void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk, ref LocalToWorld localToWorld)
            {
                int curFishIndex = chunkBaseEntityIndices[chunkIndexInQuery] + entityIndexInChunk;
                float3 couzinDirection = float3.zero;
                float3 preferredDirection;

                if (couzinDirections[curFishIndex].Seperation.Equals(float3.zero))
                {
                    couzinDirection = couzinDirections[curFishIndex].Seperation;
                }
                else if (couzinDirections[curFishIndex].Allignment.Equals(float3.zero))
                {
                    couzinDirection = couzinDirections[curFishIndex].Allignment;
                }

                preferredDirection = couzinDirection * couzinDirectionWeight
                                            + centroidFollowingDirections[curFishIndex] * centroidFollowingDirectionWeight;
                
                if (preferredDirection.Equals(float3.zero))
                {
                    preferredDirection = localToWorld.Forward;
                }
                else
                {
                    preferredDirection /= math.length(preferredDirection);
                }
                localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(
                        // TODO: precalc speed*dt
                        new float3(preferredDirection * deltaTime),
                        quaternion.LookRotationSafe(preferredDirection, math.up()),
                        new float3(1.0f, 1.0f, 1.0f))
                };
            }
        }
        //TODO :: Calculate new centroid job (not parallel but do it at the start with the first two jobs)
        //  Remember to do centroid/fishCount after finishing this job
        [BurstCompile]
        partial struct CalculateCentroidJob : IJobEntity
        {
            public float3 newCentroid;
            void Execute(in LocalToWorld localToWorld)
            {
                newCentroid = newCentroid + localToWorld.Position;
            }
        }
        //TODO :: Put new Centroid Job
        [BurstCompile]
        partial struct AssignCentroidJob : IJobEntity
        {
            [ReadOnly] public float3 newCentroid;
            void Execute(ref DynamicSchool dynamicSchool)
            {
                dynamicSchool.centroid = newCentroid;
            }
        }

        struct CouzinValues
        {
            public float3 Seperation;
            public float3 Allignment;
        }
    }
}
