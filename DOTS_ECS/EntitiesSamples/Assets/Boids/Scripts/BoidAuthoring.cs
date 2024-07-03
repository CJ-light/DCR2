using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Boids
{
    //Authoring: Holds the values that you can pass from the editor to the ECS component. Define the ECS component.
    //Baker:Attach the ECS component to the entity, and populate the ECS component with values from the authoring component. ECS component to entity.
    //ISharedComponentData: This is used to say that these values are going to be shared by all Entities in the same chunk.
    
    public class BoidAuthoring : MonoBehaviour
    {
        public float CellRadius = 8.0f;
        public float SeparationWeight = 1.0f;
        public float AlignmentWeight = 1.0f;
        public float TargetWeight = 2.0f;
        public float ObstacleAversionDistance = 30.0f;
        public float MoveSpeed = 25.0f;

        class Baker : Baker<BoidAuthoring>
        {
            // Define the Boid shared entity, and make it dynamic
            public override void Bake(BoidAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddSharedComponent(entity, new Boid
                {
                    CellRadius = authoring.CellRadius,
                    SeparationWeight = authoring.SeparationWeight,
                    AlignmentWeight = authoring.AlignmentWeight,
                    TargetWeight = authoring.TargetWeight,
                    ObstacleAversionDistance = authoring.ObstacleAversionDistance,
                    MoveSpeed = authoring.MoveSpeed
                });
            }
        }
    }

    // ISharedComponentData :: Components that are stored for multiple entities
    //  In this case, all of these components represent the school settings used for boids
    //  Stores one instance of these components to be ued for multiple entities
    //  And entities that share these components are stored next to each other in memory
    //  Bad for frequent components that you'll change frequently
    //
    // WriteGroup(typeof(LocalToWorld)) :: Lets you define dependencies, any update to the Boid componet would require an update in LocalToWorld component
    //  WriteGroup :: Lets you define what this affects, so any update done to any of these componets could potentionally affect LocalToWorld component 
    //  LocalToWorld :: Tranform matrix that turns local space coordinates to World space coordinates
    [Serializable]
    [WriteGroup(typeof(LocalToWorld))]
    public struct Boid : ISharedComponentData
    {
        public float CellRadius;
        public float SeparationWeight;
        public float AlignmentWeight;
        public float TargetWeight;
        public float ObstacleAversionDistance;
        public float MoveSpeed;
    }
}
