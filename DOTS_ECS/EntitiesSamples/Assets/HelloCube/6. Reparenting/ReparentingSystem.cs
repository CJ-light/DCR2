using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace HelloCube.Reparenting
{
    public partial struct ReparentingSystem : ISystem
    {
        bool attached;
        float timer;
        const float interval = 0.7f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            timer = interval;
            attached = true;
            state.RequireForUpdate<Execute.Reparenting>();
        }

        // Every time the timer ends, reset the timer and detach or detach the child cubes
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            timer -= SystemAPI.Time.DeltaTime;
            if (timer > 0)
            {
                return;
            }
            timer = interval;

            // get the parent singleton entity
            // Create an entityCommandBuffer :: This is used to store commands to do later
            //  This is used in order to store commmands that would change the structure of the entity chunks
            //  These commands are played later in the ecb.Playback()
            var rotatorEntity = SystemAPI.GetSingletonEntity<RotationSpeed>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // If children are attached then detach them
            // If they're detached then attach them 
            if (attached)
            {
                // Detach all children from the rotator by removing the Parent component from the children.
                // (The next time TransformSystemGroup updates, it will update the Child buffer and transforms accordingly.)

                DynamicBuffer<Child> children = SystemAPI.GetBuffer<Child>(rotatorEntity);
                for (int i = 0; i < children.Length; i++)
                {
                    // Using an ECB is the best option here because calling EntityManager.RemoveComponent()
                    // instead would invalidate the DynamicBuffer, meaning we'd have to re-retrieve
                    // the DynamicBuffer after every EntityManager.RemoveComponent() call.
                    ecb.RemoveComponent<Parent>(children[i].Value);
                }
            }
            else
            {
                // Attach all the small cubes to the rotator by adding a Parent component to the cubes.
                // (The next time TransformSystemGroup updates, it will update the Child buffer and transforms accordingly.)

                //  Query entities with tranform component, no rotation components and get entity ID
                foreach (var (transform, entity) in
                         SystemAPI.Query<RefRO<LocalTransform>>()
                             .WithNone<RotationSpeed>()
                             .WithEntityAccess())
                {
                    ecb.AddComponent(entity, new Parent { Value = rotatorEntity });
                }
            }

            // Play all the commands you stored
            ecb.Playback(state.EntityManager);

            // opposite value at the end
            attached = !attached;
        }
    }
}
