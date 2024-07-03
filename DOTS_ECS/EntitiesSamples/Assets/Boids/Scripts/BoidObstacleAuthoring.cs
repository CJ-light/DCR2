using System;
using Boids;
using Unity.Entities;
using UnityEngine;


namespace Boids
{
    public class BoidObstacleAuthoringBaker : Baker<BoidObstacleAuthoring>
    {
        public override void Bake(BoidObstacleAuthoring authoring)
        {
            //GetEntity(TransformUsageFlags): This is a function that recieves a primary entity with those flags
            //TransformUsageFlags.Renderable: This means that the component needs transform components to be rendered but its not required to move these components in run time. 
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new BoidObstacle());
        }
    }

    public struct BoidObstacle : IComponentData
    {
    }

    public class BoidObstacleAuthoring : MonoBehaviour
    {
    }
}
