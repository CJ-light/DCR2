using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DCR2
{
    public class FishAuthoring : MonoBehaviour
    {
        [Header("School caracteristics")]
        //For now, get the schoolID manually
        //TODO :: Make a unique ID for each school
        public int schoolID = 1;

        [Header("Direction Weights")]
        public float couzinDirectionWeight = 1f;
        public float centroidFollowingDirectionWeight = 2f;

        [Header("Cohesion, seperation and alignment variables")]
        public int minCentroidDistance = 10;
        public int maxCentroidDistance = 30;
        public float alpha = 2f;
        public float rho = 6f;
        public float speed = 5f;

        class Baker : Baker<FishAuthoring>
        {
            // Function to bake the entity that contains the BoidSchool component
            public override void Bake(FishAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                
                //Adds te schoolSpawn component to the entity that we just made
                AddSharedComponent(entity, new SemiStaticSchool
                {
                    schoolID = authoring.schoolID,
                    couzinDirectionWeight = authoring.couzinDirectionWeight,
                    centroidFollowingDirectionWeight = authoring.centroidFollowingDirectionWeight,
                    minCentroidDistance = authoring.minCentroidDistance,
                    maxCentroidDistance = authoring.maxCentroidDistance,
                    alpha = authoring.alpha,
                    rho = authoring.rho,
                });

                //centroid :: put the position of the spawner as the current centroid
                AddComponent(entity, new DynamicSchool
                {
                    centroid = authoring.transform.position
                });

                AddComponent(entity, new Fish{
                    speed = authoring.speed
                });
            }
        }
    }

    
    //Define the fishAuthoring components
    //semiStaticSchool :: Store class caracteristics that don't change often during run time
    //  The reason for that is because for ISharedComponentData its not advised to put values that will change often
    //  that is because it will affect the location where each fish of that class is going to be stored, which will take time processing, and if its done ofthen then that time will stack up
    [Serializable]
    [WriteGroup(typeof(LocalToWorld))]
    public struct SemiStaticSchool : ISharedComponentData
    {
        public int schoolID;
        public float couzinDirectionWeight;
        public float centroidFollowingDirectionWeight;
        public int minCentroidDistance;
        public int maxCentroidDistance;
        public float alpha;
        public float rho;
    }

    // Component that stores dynamic school data, things that are going to be changed constantly
    public struct DynamicSchool : IComponentData
    {
        public Vector3 centroid;

    }

    public struct Fish : IComponentData
    {
        public float speed;
        public bool goToCentroid;
    }
}

