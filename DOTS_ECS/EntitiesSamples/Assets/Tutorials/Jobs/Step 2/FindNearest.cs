using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Tutorials.Jobs.Step2
{

    public class FindNearest : MonoBehaviour
    {
        // The size of our arrays does not need to vary, so rather than create
        // new arrays every field, we'll create the arrays in Awake() and store them
        // in these fields.
        NativeArray<float3> TargetPositions;
        NativeArray<float3> SeekerPositions;
        NativeArray<float3> NearestTargetPositions;

        public void Start()
        {
            Spawner spawner = Object.FindObjectOfType<Spawner>();
            // Allocator.persistent :: The user is saying that the memory will be allocated until the user explicitly disposes it
            // We use the Persistent allocator because these arrays must
            // exist for the run of the program.
            TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
            SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
            NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        }

        // We are responsible for disposing of our allocations
        // when we no longer need them.
        public void OnDestroy()
        {
            TargetPositions.Dispose();
            SeekerPositions.Dispose();
            NearestTargetPositions.Dispose();
        }

        public void Update()
        {
            // Copy every target transform to a NativeArray.
            for (int i = 0; i < TargetPositions.Length; i++)
            {
                // Vector3 is implicitly converted to float3
                TargetPositions[i] = Spawner.TargetTransforms[i].localPosition;
            }

            // Copy every seeker transform to a NativeArray.
            for (int i = 0; i < SeekerPositions.Length; i++)
            {
                // Vector3 is implicitly converted to float3
                SeekerPositions[i] = Spawner.SeekerTransforms[i].localPosition;
            }

            // To schedule a job, we first need to create an instance and populate its fields.
            // Accessing the FindNearestJob IJob class and instantiating it so that we can run it
            // This job updates the Nearest Target Positoins which we will use to draw lines from seekers to the newarst target.
            FindNearestJob findJob = new FindNearestJob
            {
                TargetPositions = TargetPositions,
                SeekerPositions = SeekerPositions,
                NearestTargetPositions = NearestTargetPositions,
            };

            // Schedule() puts the job instance on the job queue.
            // It will run the execute() function on the FindNearestJob 
            JobHandle findHandle = findJob.Schedule();

            // The Complete method will not return until the job represented by
            // the handle finishes execution. Effectively, the main thread waits
            // here until the job is done.
            findHandle.Complete();

            // Draw a debug line from each seeker to its nearest target.
            for (int i = 0; i < SeekerPositions.Length; i++)
            {
                // float3 is implicitly converted to Vector3
                Debug.DrawLine(SeekerPositions[i], NearestTargetPositions[i]);
            }
        }
    }
}
