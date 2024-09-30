using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;

//These are used to enable test things like Debug.Log()
using UnityEngine;
using Unity.Rendering;

//Summary :: Once the SchoolSpawner controller entity updates, it spawns its corresponding fish,
//  afterwards, this entity is deleted so that it doesn't spawn again
namespace DCR2
{
    // RequireMatcingQueriesForUpdates :: Skips the OnUpdate system if there are no entities found in the EntityQueries that you do
    //  Basically, this doens't run OnUpdate until there are entities that match the quesries done in this system (Until we've defined our entity spawner)
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct CentroidGigzmoSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var centroidQuery = SystemAPI.QueryBuilder().WithAll<centroidGizmoComponent>().WithAll<LocalToWorld>().Build();
            if (centroidQuery.CalculateEntityCount() > 0)
            {
                var fishQuery = SystemAPI.QueryBuilder().WithAll<DynamicSchool>().Build();
                NativeArray<Entity> entityArray = fishQuery.ToEntityArray(Allocator.TempJob);
                //var centroidVal = state.EntityManager.GetComponentData<DynamicSchool>(entityArray[0]).centroid;

                var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
                //I think this could be done in the SchoolSpawner (instantiating the centroids)
                //Array to store the amount of gizmos
                //TODO :: Right now it only stores 1 entity, it should store the same amount as the amount of schoolSpawners

                //Instantiate the sphere prefab and put it in the centroidGizmoArray
                //state.EntityManager.Instantiate(centerGizmo.ValueRO.spherePrefab);
                //Debug.Log("In Onpudate");
                NativeArray<Entity> centroidEntityArray = centroidQuery.ToEntityArray(Allocator.TempJob);

                //localToWorldLookup[entityArray[0]] = localToWorld;
                //Debug.Log(localToWorldLookup[entityArray[0]].Position);
                //?? its spawning the entity but I cant see it???
                //Debug.Log("Centroid location calc");
                //Debug.Log(centroidQuery.CalculateEntityCount());

                
                //Put the entity on the position of the centroid of one of the schools
                var localToWorld = new LocalToWorld
                        {
                            Value = float4x4.TRS(localToWorldLookup[entityArray[0]].Position, quaternion.LookRotationSafe(new float3(0f,0f,0f), math.up()), new float3(10.0f, 10.0f, 10.0f))
                        };
                localToWorldLookup[centroidEntityArray[0]] = localToWorld;
            }
        }
    }
}

