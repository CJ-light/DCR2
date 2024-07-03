using Unity.Entities;
using UnityEngine;

namespace HelloCube.Prefabs
{
    // An authoring component is just a normal MonoBehavior that has a Baker<T> class.
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject Prefab;

        // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
        // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
        // is simply an optional matter of style.)
        class Baker : Baker<SpawnerAuthoring>
        {

            public override void Bake(SpawnerAuthoring authoring)
            {
                // Spawner will not move and that's why it has non transform flags
                // Declare a new component called spawner, this will store a prefab inside of it
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Spawner
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    // Declare the structure of the Spawner IComponent, which will only contain the prefab of the cube we want to spawn
    struct Spawner : IComponentData
    {
        public Entity Prefab;
    }
}
