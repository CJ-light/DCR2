using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;
using UnityEngine;

namespace Boids
{
    // RequireMatcingQueriesForUpdates :: Skips the OnUpdate system if there are no entities found in the EntityQueries that you do
    //  Basically, this doens't run OnUpdate until there are entities that match the quesries done in this system (Until we've defined our entity spawner)
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct BoidSchoolSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            //ecb (Entity Command Buffer): This buffer acts as a handle to the EntityManager, the EntityManager can create or remove entities; add, change or remove copmonents.
            //  The problem with doing these types of command in a for loop is that these commands can affect the structure of the list we're iterating, if we change it while iterating we might get some bugs
            //  Or it might take longer to do than doing it after the for loop is done.
            //Allocator.Temp: Memory is allocated to this value for a temporary amount of time, around 4 frames of time.

            Debug.Log("OnUpdate");
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var world = state.World.Unmanaged;

            //Query to get boidSchools, localtoworld transform and the boidSchool entity id
            //RefRO: Read only these components
            //.WithEntityAccess(): Give direct access to these entities
            //Searches for the all the entities that contain the BoidSchool and LocalToWorld Components, and also returns the entity ID fo each one 
            foreach (var (boidSchool, boidSchoolLocalToWorld, entity) in
                     SystemAPI.Query<RefRO<BoidSchool>, RefRO<LocalToWorld>>()
                         .WithEntityAccess())
            {
                //214 Spawns real fish
                //258 Spawns ghost fish
                if (entity.Index == 214)
                {
                    //Debug.Log("Getting out of loop");
                    //ecb.DestroyEntity(entity);
                    return;
                }
                Debug.Log("In for loop");
                                Debug.LogFormat("School: {0}, SchoolLocalToWorld {1}, Entity: {2}", boidSchool.ValueRO, boidSchoolLocalToWorld.ValueRO, entity);

                //CreateNativeArray<T>(NativeArray<T> array, AllocatorManager.AllocatorHandle allocator)
                //  This creates a copy of the other native array with the allocator
                //  so it returns a List of the amount of individuals for each school
                //      Allocator is what lets us know how long that memory should last
                //          world.UpdateAllocator: Do not need to be explicitly disposed, all alocations made here last around two full world updates
                //Rewindable Allocator :: dynamic memory, doubles block of memory it can use 
                //Define boidsEntities as an array with boidSchool.ValueRO.Count amount of spaces in the array, right now its an empty array.
                var boidEntities =
                    CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(boidSchool.ValueRO.Count,
                        ref world.UpdateAllocator);

                //Creates clones of the boidSchool Prefabs and stores them into the boidEntities array
                //  Note: boidSchool has prefabs for the fish that it spawns, the prefabs are of boids type
                state.EntityManager.Instantiate(boidSchool.ValueRO.Prefab, boidEntities);

                // Instantiate the SetBoidLocalToWorld job
                //  This job moves the fish entities in boidEntities into random positions
                var setBoidLocalToWorldJob = new SetBoidLocalToWorld
                {
                    LocalToWorldFromEntity = localToWorldLookup,
                    Entities = boidEntities,
                    Center = boidSchoolLocalToWorld.ValueRO.Position,
                    Radius = boidSchool.ValueRO.InitialRadius
                };

                //Schuedule:: Schedule job
                //  1: Number of iterations in the job
                //  2: Job batch size
                //  3: Define that you put it in the state.Dependency queue(kinda), basically, if you have another job in there beforehand then it does that job first and then this one
                //      so its kind of like a queue for jobs
                state.Dependency = setBoidLocalToWorldJob.Schedule(boidSchool.ValueRO.Count, 64, state.Dependency);
                
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
    struct SetBoidLocalToWorld : IJobParallelFor
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

        // Execute :: Built in function for jobs which is looped for each value in the school List
        public void Execute(int i)
        {
            // entity :: the entity we want to spawn
            // random :: We use this vairable to create a random seed
            // dir :: create a random direction
            // pos :: create a random position
            // localToWorld :: Define the position and direction into the world coordinates
            // Define the random position and direction of each fish spawned beforehand. 
            var entity = Entities[i];
            var random = new Unity.Mathematics.Random(((uint)(entity.Index + i + 1) * 0x9F6ABC1));
            var dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
            var pos = Center + (dir * Radius);
            var localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(pos, quaternion.LookRotationSafe(dir, math.up()), new float3(1.0f, 1.0f, 1.0f))
            };
            LocalToWorldFromEntity[entity] = localToWorld;
        }
    }
}
