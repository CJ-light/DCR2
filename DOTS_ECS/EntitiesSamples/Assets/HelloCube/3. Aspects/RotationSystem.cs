using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HelloCube.Aspects
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.Aspects>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            double elapsedTime = SystemAPI.Time.ElapsedTime;

            // Rotate the cube directly without using the aspect.
            // The query matches all entities having the LocalTransform and RotationSpeed components.
            foreach (var (transform, speed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.RadiansPerSecond * deltaTime);
            }

            // Rotate the cube using the aspect.
            // The query will include all components of VerticalMovementAspect.
            // VerticalMovementAspect does a similar query as we did before, it looks up things with the transform and speed components in our script
            // Note that, unlike components, aspect type params of SystemAPI.Query are not wrapped in a RefRW or RefRO.
            foreach (var movement in
                     SystemAPI.Query<VerticalMovementAspect>())
            {
                movement.Move(elapsedTime);
            }
        }
    }

    // An instance of this aspect wraps the LocalTransform and RotationSpeed components of a single entity.
    // (This trivial example is arguably not a worthwhile use case for aspects, but larger examples better demonstrate their utility.)
    readonly partial struct VerticalMovementAspect : IAspect
    {
        // readonly: This keyword indicates that the field m_Transform can only be assigned a value during its initialization or in the constructor of the class where it is declared. Once assigned, its value cannot be changed elsewhere in the code.
        // RefRW reference to the localtransform component with Read write flag
        readonly RefRW<LocalTransform> m_Transform;
        readonly RefRO<RotationSpeed> m_Speed;

        // Fucntion of this aspect
        public void Move(double elapsedTime)
        {
            m_Transform.ValueRW.Position.y = (float)math.sin(elapsedTime * m_Speed.ValueRO.RadiansPerSecond);
        }
    }
}
