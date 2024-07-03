using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HelloCube.Prefabs
{
    public partial struct FallAndDestroySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // Wait for it to have prefabs in the scene before excecuting the OnUpdate
            state.RequireForUpdate<Execute.Prefabs>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // rotation
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, speed) in
                    // Find all obejects thhat have LocalTranfrom and RotationSpeed Components (The cubes)
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
            {
                // ValueRW and ValueRO both return a ref to the actual component value.
                // The difference is that ValueRW does a safety check for read-write access while
                // ValueRO does a safety check for read-only access.
                transform.ValueRW = transform.ValueRO.RotateY(
                    speed.ValueRO.RadiansPerSecond * deltaTime);
            }

            // I think they use the buffer to put entities that should be deleted on the next update, they need to be stored because you cant do that while in the for query because it changes the structure of the native array, which might give you problems if youre iterating throught it
            // An EntityCommandBuffer created from EntityCommandBufferSystem.Singleton will be
            // played back and disposed by the EntityCommandBufferSystem when it next updates.
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // Downward vector
            var movement = new float3(0, -SystemAPI.Time.DeltaTime * 5f, 0);

            // Move the cubes down and destoy them once they go below a y axis of 0

            // WithAll() includes RotationSpeed in the query, but
            // the RotationSpeed component values will not be accessed.
            // WithEntityAccess() includes the Entity ID as the last element of the tuple.
            foreach (var (transform, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<RotationSpeed>()
                         .WithEntityAccess())
            {
                transform.ValueRW.Position += movement;
                // If the cubes have gone below a y axis of 0 then destroy them, another ucbe will be spanwed later by the spawner
                if (transform.ValueRO.Position.y < 0)
                {
                    // Making a structural change would invalidate the query we are iterating through,
                    // so instead we record a command to destroy the entity later.
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}
