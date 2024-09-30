using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;

using UnityEngine;
using Unity.Rendering;

namespace DCR2
{
    // RequireMatcingQueriesForUpdates :: Skips the OnUpdate system if there are no entities found in the EntityQueries that you do
    //  Basically, this doens't run OnUpdate until there are entities that match the quesries done in this system (Until we've defined our entity spawner)
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct TestSchoolSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            //ecb (Entity Command Buffer): This buffer acts as a handle to the EntityManager, the EntityManager can create or remove entities; add, change or remove copmonents.
            //  The problem with doing these types of command in a for loop is that these commands can affect the structure of the list we're iterating, if we change it while iterating we might get some bugs
            //  Or it might take longer to do than doing it after the for loop is done.
            //Allocator.Temp: Memory is allocated to this value for a temporary amount of time, around 4 frames of time.

            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var world = state.World.Unmanaged;
            var testSpawnComponents = SystemAPI.GetComponentLookup<TestSpawn>();

            //RefRO: Read only these components
            //.WithEntityAccess(): Give direct access to these entities
            //Searches for the all the entities that contain the BoidSchool and LocalToWorld Components, and also returns the entity ID fo each one 
            foreach (var (school, schoolLocalToWorld, testSpawn, entity) in
                     SystemAPI.Query<RefRW<SchoolSpawn>, RefRO<LocalToWorld>, RefRO<TestSpawn>>()
                         .WithEntityAccess())
            {
                /*NativeArray<ComponentType> componentTypes = state.EntityManager.GetComponentTypes(school.ValueRO.schoolPrefab, Allocator.Temp);
                for (int i = 0; i < componentTypes.Length; i++)
                {
                    Debug.LogFormat("Component #{0} of gameObject: {1}", i, componentTypes[i]);
                    if (i == 18)
                    {
                        //Because its a shared component you need to use the GetSharedComponentManaged function instead of simply GetComponent function
                        Debug.LogFormat("Mesh Array: {0}", state.EntityManager.GetSharedComponentManaged<RenderMeshArray>(school.ValueRO.schoolPrefab).ComputeHash128());
                    }
                }*/
                //List<ComponentType> componentTypes = EntityManager.GetComponentTypes(school.ValueRO.schoolPrefab, RewindableAllocator);
                if (entity.Index == 204)
                {
                    //Debug.Log("Getting out of loop");
                    ecb.DestroyEntity(entity);
                    return;
                }

                float4x4 matrix = schoolLocalToWorld.ValueRO.Value;
                float3 translation = matrix.c3.xyz;
                quaternion rotation = new quaternion(matrix);
                float3 scale = new float3(math.length(matrix.c0.xyz), math.length(matrix.c1.xyz), math.length(matrix.c2.xyz));
                /*
                Debug.LogFormat("School: {0}, SchoolLocalToWorld {1}, Entity: {2}", school.ValueRO, schoolLocalToWorld.ValueRO, entity);
                Debug.LogFormat("School -> SpawnRadius: {0}, SpawnCount: {1} :: LocalToWorld -> Position: {2}, Rotation: {3}, Scale: {4}, InstanceID: {5}",
                    school.ValueRO.spawnRadius,
                    school.ValueRO.spawnCount,
                    translation,
                    rotation,
                    scale,
                    school.ValueRO.schoolPrefab);
                */
                //CreateNativeArray<T>(NativeArray<T> array, AllocatorManager.AllocatorHandle allocator)
                //  This creates a copy of the other native array with the allocator
                //  so it returns a List of the amount of individuals for each school
                //      Allocator is what lets us know how long that memory should last
                //          world.UpdateAllocator: Do not need to be explicitly disposed, all alocations made here last around two full world updates
                //Rewindable Allocator :: dynamic memory, doubles block of memory it can use 
                //Define boidsEntities as an array with boidSchool.ValueRO.Count amount of spaces in the array, right now its an empty array.
                var schoolEntities =
                    CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(school.ValueRO.spawnCount,
                        ref world.UpdateAllocator);

                //Creates clones of the boidSchool Prefabs and stores them into the boidEntities array
                //  Note: boidSchool has prefabs for the fish that it spawns, the prefabs are of boids type
                state.EntityManager.Instantiate(school.ValueRO.schoolPrefab, schoolEntities);

                // Instantiate the SetBoidLocalToWorld job
                //  This job moves the fish entities in boidEntities into random positions
                var TestSetSchoolLocalToWorldJob = new TestSetSchoolLocalToWorld
                {
                    LocalToWorldFromEntity = localToWorldLookup,
                    Entities = schoolEntities,
                    Center = schoolLocalToWorld.ValueRO.Position,
                    Radius = school.ValueRO.spawnRadius,
                    spawnType = testSpawn.ValueRO.spawnType,
                    minMagnitude = testSpawn.ValueRO.minMagnitude,
                    maxMagnitude = testSpawn.ValueRO.maxMagnitude,
                    forwardDis = testSpawn.ValueRO.forwardDis,
                    horizontalDis = testSpawn.ValueRO.horizontalDis,
                };

                //Schuedule:: Schedule job
                //  1: Number of iterations in the job
                //  2: Job batch size
                //  3: Define that you put it in the state.Dependency queue(kinda), basically, if you have another job in there beforehand then it does that job first and then this one
                //      so its kind of like a queue for jobs
                
                //Debug.LogFormat("School spawn count: {0}", school.ValueRO.spawnCount);
                
                state.Dependency = TestSetSchoolLocalToWorldJob.Schedule(school.ValueRO.spawnCount, 64, state.Dependency);
                
                //state.Dependency.Complete() :: waits for all jobs to complete in order to go to th next line
                state.Dependency.Complete();

                //puts the Deletes the the entity command into a queue
                ecb.DestroyEntity(entity);
            }

            //Deletes the entity ID's after the for loop is done
            ecb.Playback(state.EntityManager);
        }
    }
    
    [BurstCompile]
    struct TestSetSchoolLocalToWorld : IJobParallelFor
    {
        //[NativeDisableContainerSafetyRestriction] :: This disables safety checks on the system, gives you errors if it finds some
        // If the game has an error the game just crashes
        //[NativeDisableParallelForRestriction] :: In the same way this one disables safety checks for parralel processing
        [NativeDisableContainerSafetyRestriction] [NativeDisableParallelForRestriction]
        
        // Looks up the LocalToWorld component in the LocalToWorldFromEntity entity
        public ComponentLookup<LocalToWorld> LocalToWorldFromEntity;

        // Entities :: Array of the school prefab
        // Center :: center of where the fish are going to spawn
        // Radius :: Spawn radius
        public NativeArray<Entity> Entities;
        public float3 Center;
        public float Radius;
        public int spawnType;
        public int minMagnitude;
        public int maxMagnitude;
        public int forwardDis;
        public int horizontalDis;

        // Execute :: Built in function for jobs which is looped for each value in the school List
        public void Execute(int i)
        {
            // entity :: the entity we want to spawn
            // random :: We use this vairable to create a random seed
            // dir :: create a random direction
            // pos :: create a random position
            // localToWorld :: Define the position and direction into the world coordinates
            // Define the random position and direction of each fish spawned beforehand. 

            float3 dir = new float3(0);
            float3 pos = new float3(0);
            var entity = Entities[i];
            var random = new Unity.Mathematics.Random(((uint)(entity.Index + i + 1) * 0x9F6ABC1));

            //SpawnType == 1 :: Random spawn with magnitude
            //This type of spawn puts every fish in a random position and then puts them farther away with a random magnitude
            if (spawnType == 1)
            {
                dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
                var randPos = Radius * math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
                var randDist = random.NextFloat3(minMagnitude, maxMagnitude);
                pos = Center + (dir * randPos * randDist);
            }
            //SpawnType == 2 :: Vertical line spawn
            //Spawn the fish in a vertical line, where each fish has a specific distance between them
            else if (spawnType == 2)
            {
                dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
                pos = Center + new float3(0f, 0f, ((forwardDis/2 + math.floor(i/2))*forwardDis) * math.pow(-1,i));
            }
            //TODO :: Find out why it doesnt work as expected
            //SpawnType == 3 :: Two vertical line spawn
            //Spawn two vertical lines of fish
            else if (spawnType == 3)
            {
                dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
                pos = Center + new float3(horizontalDis/2 * math.pow(-1, math.floor(i/2)), 0f, (forwardDis/2 + (math.floor(i/2))*forwardDis) * math.pow(-1,i));
            }
            var localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(pos, quaternion.LookRotationSafe(dir, math.up()), new float3(1.0f, 1.0f, 1.0f))
            };
            LocalToWorldFromEntity[entity] = localToWorld;
        }
    }
}

