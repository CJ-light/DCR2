using Unity.Entities;
using UnityEngine;

namespace BoidsSpawn
{
    public class BoidSchoolAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public float InitialRadius;
        public int Count;

        class Baker : Baker<BoidSchoolAuthoring>
        {
            // Function to bake the entity that contains the BoidSchool component
            public override void Bake(BoidSchoolAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                
                //Adds te BoidSchool component to the entity that we just made
                AddComponent(entity, new BoidSchool
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                    Count = authoring.Count,
                    InitialRadius = authoring.InitialRadius
                });
            }
        }
    }

    //IComponentData: This is an interace used for general-purpose components that are almost always giong to be accessed or almost always at the same time
    // This entity is the one that will have the prefab, of the fish, the initial radius on where to spawn and the amount of fish to spawn. 
    public struct BoidSchool : IComponentData
    {
        public Entity Prefab;
        public float InitialRadius;
        public int Count;
    }
}
