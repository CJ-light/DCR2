using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HelloCube.Prefabs
{
    public partial struct SpawnSystem : ISystem
    {
        uint updateCounter;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // This call makes the system not update unless at least one entity in the world exists that has the Spawner component.
            state.RequireForUpdate<Spawner>();

            state.RequireForUpdate<Execute.Prefabs>();
        }

        // Spawns 500 cubes at random lcoations, then they fall alone
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Create a query that matches all entities having a RotationSpeed component.
            // (The query is cached in source generation, so this does not incur a cost of recreating it every update.)
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeed>().Build();

            // Only spawn cubes when no cubes currently exist.
            if (spinningCubesQuery.IsEmpty)
            {
                // Will return the entity that contains the spawner component
                // If there are no entities with a spawner component or if there are mutliple entities with a spawner component then this will return an error
                var prefab = SystemAPI.GetSingleton<Spawner>().Prefab;

                // Instantiating an entity creates copy entities with the same component types and values.
                // Spawn 500 cubes (prefabs) and only store this information for this function (Allocator.Temp)
                // They will still be in the scene even if we delete them from this class.
                // This function returns all the entity ID's of the entities we just made
                var instances = state.EntityManager.Instantiate(prefab, 500, Allocator.Temp);

                // Unlike new Random(), CreateFromIndex() hashes the random seed
                // so that similar seeds don't produce similar results.
                // so its just a more random random
                var random = Random.CreateFromIndex(updateCounter++);

                // For each entity in instances put a random position for them
                // 
                foreach (var entity in instances)
                {
                    // Its looking up the component at each entity
                    // This is actually more expensive than looking up through the query, since queries already have all components of the same type next to each other this is more expensive to do because you're looking up a random location for that component
                    //  Something like that...
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);

                    // Update the entity's LocalTransform component with the new random position.
                    transform.ValueRW.Position = (random.NextFloat3() - new float3(0.5f, 0, 0.5f)) * 20;
                }
            }
        }
    }
}
