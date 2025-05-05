using UnityEngine;

public class UIPanelCollisionReporter : MonoBehaviour
{
    public UIPanelAgent_StayInBounds agent;

    private void OnTriggerEnter(Collider other)
    {
        if (agent == null) return;

        if (other == agent.roomBoundsCollider)
        {
            agent.SetInRoomBounds(true);
            Debug.Log("[TRIGGER] Entered room bounds");
        }
        else if (other == agent.garageBoundsCollider)
        {
            agent.SetInGarageBounds(true);
            Debug.Log("[TRIGGER] Entered garage bounds");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (agent == null) return;

        if (other == agent.roomBoundsCollider)
        {
            agent.SetInRoomBounds(false);
            Debug.Log("[TRIGGER] Exited room bounds");
        }
        else if (other == agent.garageBoundsCollider)
        {
            agent.SetInGarageBounds(false);
            Debug.Log("[TRIGGER] Exited garage bounds");
        }
    }
}
