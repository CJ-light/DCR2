using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DCR2
{
    public class SchoolSpawnAuthoring : MonoBehaviour
    {
        public GameObject schoolPrefab;
        public float spawnRadius = 6f;
        public int spawnCount = 100;

        class Baker : Baker<SchoolSpawnAuthoring>
        {
            // Function to bake the entity that contains the BoidSchool component
            public override void Bake(SchoolSpawnAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                
                //Adds te schoolSpawn component to the entity that we just made
                AddComponent(entity, new SchoolSpawn
                {
                    schoolPrefab = GetEntity(authoring.schoolPrefab, TransformUsageFlags.Dynamic),
                    spawnCount = authoring.spawnCount,
                    spawnRadius = authoring.spawnRadius
                });
            }
        }
    }

    //IComponentData: This is an interace used for general-purpose components that are almost always giong to be accessed or almost always at the same time
    // This entity is the one that will have the prefab, of the fish, the initial radius on where to spawn and the amount of fish to spawn. 
    //Define the schoolSpawn component

    //*Why use public struct instead of a class
    public struct SchoolSpawn : IComponentData
    {
        public Entity schoolPrefab;
        public float spawnRadius;
        public int spawnCount;
    }
}
