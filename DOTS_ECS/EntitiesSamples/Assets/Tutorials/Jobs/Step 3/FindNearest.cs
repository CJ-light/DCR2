using Spawner = Tutorials.Jobs.Step2.Spawner;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Tutorials.Jobs.Step3
{
    // Exactly the same as in prior step except this schedules a parallel job instead of a single-threaded job.
    public class FindNearest : MonoBehaviour
    {
        NativeArray<float3> TargetPositions;
        NativeArray<float3> SeekerPositions;
        NativeArray<float3> NearestTargetPositions;

        //Spawner Game object is using the spawner script from another step to instantiate the seekers and targets
        //  GetComponent:: Its getting the Spawner script component, and using it to get the info of what it spawns
        public void Start()
        {
            Spawner spawner = GetComponent<Spawner>();
            TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
            SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
            NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        }

        //Make sure to remove anything that you make with Allocator.Persistent
        public void OnDestroy()
        {
            TargetPositions.Dispose();
            SeekerPositions.Dispose();
            NearestTargetPositions.Dispose();
        }

        public void Update()
        {
            //Use main thread to update the target positions.
            for (int i = 0; i < TargetPositions.Length; i++)
            {
                TargetPositions[i] = Spawner.TargetTransforms[i].localPosition;
            }

            for (int i = 0; i < SeekerPositions.Length; i++)
            {
                SeekerPositions[i] = Spawner.SeekerTransforms[i].localPosition;
            }

            //Making the instantiation of the FindNearestJob class
            FindNearestJob findJob = new FindNearestJob
            {
                TargetPositions = TargetPositions,
                SeekerPositions = SeekerPositions,
                NearestTargetPositions = NearestTargetPositions,
            };

            // Execute will be called once for every element of the SeekerPositions array,
            // with every index from 0 up to (but not including) the length of the array.
            // The Execute calls will be split into batches of 100.
            JobHandle findHandle = findJob.Schedule(SeekerPositions.Length, 100);

            findHandle.Complete();

            //Use the main thread to draw these lines, this is after calculating the positoin of the lines through jobs
            for (int i = 0; i < SeekerPositions.Length; i++)
            {
                Debug.DrawLine(SeekerPositions[i], NearestTargetPositions[i]);
            }
        }
    }
}

