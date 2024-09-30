/*using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DCR2
{
    [RequireMatchingQueriesForUpdate]
    public partial struct GizmoSystem : ISystem
    {
        //Trying to make it work with just 1 school
        //EntityQuery query;

        public void OnUpdate(ref SystemState state)
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
            var centroidQuery = SystemAPI.QueryBuilder().WithAll<centroidGizmoComponent>().WithAll<LocalToWorld>().Build();
            NativeArray<Entity> centroidEntityArray = centroidQuery.ToEntityArray(Allocator.TempJob);

            //?? its spawning the entity but I cant see it???
            //Debug.Log(centroidQuery.CalculateEntityCount());
            var localToWorld = new LocalToWorld
                    {
                        Value = float4x4.TRS(new float3(0f,0f,0f), quaternion.LookRotationSafe(new float3(0f,0f,0f), math.up()), new float3(1.0f, 1.0f, 1.0f))
                    };
            localToWorldLookup[centroidEntityArray[0]] = localToWorld;
            //Debug.Log(localToWorldLookup[centroidEntityArray[0]]);
            /*foreach (var (centerGizmo, entity) in
                     SystemAPI.Query<RefRW<centroidGizmo>>()
                         .WithEntityAccess())
                {
                    Debug.Log("SettingLocation");
                    var localToWorld = new LocalToWorld
                    {
                        Value = float4x4.TRS(new float3(0f,0f,0f), quaternion.LookRotationSafe(new float3(0f,0f,0f), math.up()), new float3(1.0f, 1.0f, 1.0f))
                    };
                    localToWorldLookup[entity] = localToWorld;
                }

        }
    }
}*/
