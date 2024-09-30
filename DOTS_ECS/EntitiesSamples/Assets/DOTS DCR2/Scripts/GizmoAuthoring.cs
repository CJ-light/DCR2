using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DCR2
{
    public class GizmoAuthoring : MonoBehaviour
    {
        public GameObject centroidPrefab;
        public int showCentroid;

        class Baker : Baker<GizmoAuthoring>
        {
            // Function to bake the entity that contains the BoidSchool component
            // authoring :: This is the value that has the inputs that the user gave 
            public override void Bake(GizmoAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                
                if (authoring.showCentroid == 1)
                {
                    AddComponent(entity, new centroidGizmo
                    {
                        centroidPrefab = GetEntity(authoring.centroidPrefab, TransformUsageFlags.Dynamic),
                    });
                }
            }
        }
    }

    
    //Define each of the components that apply to the fish
    //semiStaticSchool :: Store class caracteristics that don't change often during run time
    //  The reason for that is because for ISharedComponentData its not advised to put values that will change often
    //  that is because it will affect the location where each fish of that class is going to be stored, which will take time processing, and if its done ofthen then that time will stack up
    //
    [Serializable]
    [WriteGroup(typeof(LocalToWorld))]
    public struct centroidGizmo : IComponentData
    {
        public Entity centroidPrefab;
    }
}

