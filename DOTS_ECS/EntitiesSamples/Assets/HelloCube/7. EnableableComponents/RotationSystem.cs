using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace HelloCube.EnableableComponents
{
    public partial struct RotationSystem : ISystem
    {
        float timer;
        const float interval = 1.3f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            timer = interval;
            state.RequireForUpdate<Execute.EnableableComponents>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            timer -= deltaTime;

            // Toggle the enabled state of every RotationSpeed
            if (timer < 0)
            {
                //  Do a query of entities with RotationSpeed that is enabled (withOptions also gives us the non-enabled one)
                //  Toggle the enalbed and disbled rotation
                foreach (var rotationSpeedEnabled in
                         SystemAPI.Query<EnabledRefRW<RotationSpeed>>()
                             .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                {
                    rotationSpeedEnabled.ValueRW = !rotationSpeedEnabled.ValueRO;
                }

                //  Checking to disable or enable after an interval
                timer = interval;
            }

            // The query only matches entities whose RotationSpeed is enabled. (It's already a given when you do a query)
            foreach (var (transform, speed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(
                    speed.ValueRO.RadiansPerSecond * deltaTime);
            }
        }
    }
}
