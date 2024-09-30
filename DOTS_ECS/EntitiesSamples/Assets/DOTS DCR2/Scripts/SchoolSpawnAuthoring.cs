using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DCR2
{
    public class SchoolSpawnAuthoring : MonoBehaviour
    {
        public GameObject schoolPrefab;
        public float spawnRadius;
        public int spawnCount;

        [Header("Test spawn variables, keep on 0 if you dont want to change the spawn types")]
        public int spawnType;

        [Header("SpawnType: 1 -> This is one where it does the normal calculation but it multiplies the resulting direction by a random magnitude")]
        public int minMagnitude;
        public int maxMagnitude;

        [Header("SpawnType: 2 -> This spawn type spawns the fish in a line with a specific distance between them")]
        public int forwardDis;

        [Header("SpawnType: 3 -> This type spawns two lines of fish, the distance between the two lines is the horizontal Distance")]
        public int horizontalDis;

        /*//TODO :: Instead of doing it like this, figure out if you can instead add the gizmo as a component in runtime, instead of adding it here in the authoring (Create an entity of the gizmo-> put a flag in the entity to show or not to show gizmos -> wirte a gizmo system that looks up all the gizmos used and how they're going to move.)
        [Header("Fake Gizmo: Show centroid")]
        public GameObject spherePrefab;
        public int showCentroid;*/

        class Baker : Baker<SchoolSpawnAuthoring>
        {
            // Function to bake the entity that contains the BoidSchool component
            public override void Bake(SchoolSpawnAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                
                //Adds the schoolSpawn component to the entity that we just made
                AddComponent(entity, new SchoolSpawn
                {
                    schoolPrefab = GetEntity(authoring.schoolPrefab, TransformUsageFlags.Dynamic),
                    spawnCount = authoring.spawnCount,
                    spawnRadius = authoring.spawnRadius,
                });

                if (authoring.spawnType != 0)
                {
                    AddComponent(entity, new TestSpawn
                    {
                        spawnType = authoring.spawnType,
                        minMagnitude = authoring.minMagnitude,
                        maxMagnitude = authoring.maxMagnitude,
                        forwardDis = authoring.forwardDis,
                        horizontalDis = authoring.horizontalDis,
                    });
                }

                /*if (authoring.GizmoCentroid != 0)
                {
                    AddComponent(entity, new GizmoCentroid
                    {
                        GizmoCentroid = authoring.GizmoCentroid,
                    });
                }*/
                /*AddComponent(entity, new TestSpawn
                {
                    spawnType = authoring.spawnType,
                });*/
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

    public struct TestSpawn : IComponentData
    {
        public int spawnType;
        public int minMagnitude;
        public int maxMagnitude;
        public int forwardDis;
        public int horizontalDis;
    }

    /*public struct GizmoCentroid : IComponentData
    {
        public Entity spherePrefab;
    }*/
}
