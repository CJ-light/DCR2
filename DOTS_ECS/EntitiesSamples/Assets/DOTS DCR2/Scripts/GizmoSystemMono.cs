/*using Unity.Entities;
using UnityEngine;

public class GizmoSystemMono : MonoBehaviour
{

    private void Start()
    {
    }

    private void OnDrawGizmos(ref SystemState state)
    {
        var systemHandler = World.GetExistingSystem<GizmoSystem>();
        var system = state.WorldUnmanaged.GetUnsafeSystemRef<GizmoSystem>(systemHandler);
        if (system != null)
        {
            var centroidVal = system.GizmoPosition();
            Gizmos.DrawSphere(centroidVal, 5f);
        }
    }
}
*/
