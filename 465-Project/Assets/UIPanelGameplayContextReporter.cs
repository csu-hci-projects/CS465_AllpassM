using UnityEngine;

public class UIPanelGameplayContextReporter : MonoBehaviour
{
    public UIPanelAgent_GameplayContext agent;

    private void OnTriggerEnter(Collider other)
    {
        if (agent == null) return;

        if (other == agent.puzzleWallTriggerZone)
        {
            agent.SetInPuzzleWallZone(true);
            Debug.Log("[TRIGGER] Entered PuzzleWallTriggerZone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (agent == null) return;

        if (other == agent.puzzleWallTriggerZone)
        {
            agent.SetInPuzzleWallZone(false);
            Debug.Log("[TRIGGER] Exited PuzzleWallTriggerZone");
        }
    }
}
