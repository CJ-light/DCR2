using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HelloCube.EnableableComponents
{
    public class RotationSpeedAuthoring : MonoBehaviour
    {
        public bool StartEnabled;
        public float DegreesPerSecond = 360.0f;

        public class Baker : Baker<RotationSpeedAuthoring>
        {
            public override void Bake(RotationSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                // Add Rotation speed component
                // Set rotation speed as a component you can enable and disable
                AddComponent(entity, new RotationSpeed { RadiansPerSecond = math.radians(authoring.DegreesPerSecond) });
                SetComponentEnabled<RotationSpeed>(entity, authoring.StartEnabled); 
            }
        }
    }

    // IEnableComponents lets us say tht we can enable and disable it
    struct RotationSpeed : IComponentData, IEnableableComponent
    {
        public float RadiansPerSecond;
    }
}
