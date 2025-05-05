using UnityEngine;

public class PlayerPuzzleWallTriggerReporter : MonoBehaviour
{
    public UIPanelAgent_GameplayContext agent;
    public Collider puzzleWallTriggerZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other == puzzleWallTriggerZone)
        {
            agent.SetInPuzzleWallZone(true);
            Debug.Log("[TRIGGER] Player entered puzzle wall zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == puzzleWallTriggerZone)
        {
            agent.SetInPuzzleWallZone(false);
            Debug.Log("[TRIGGER] Player exited puzzle wall zone");
        }
    }
}
