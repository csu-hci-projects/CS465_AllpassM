using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class UIPanelAgent_GameplayContext : Agent
{

    //Vector observations this agent has access to
    public Transform uiPanel;
    public Transform playerHead;
    public Collider puzzleWallTriggerZone;

    private Rigidbody panelRigidbody;
    private bool isInPuzzleWallZone = false;
    private bool keycardPickedUp = false;


    public override void Initialize()
    {
        panelRigidbody = uiPanel.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(uiPanel.position);
        sensor.AddObservation(playerHead.position);
        sensor.AddObservation(playerHead.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {   
        //Allows the agent to move along the x,y,z axis
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);
        Vector3 move = new Vector3(moveX, moveY, moveZ) * 2f;
        Vector3 newPosition = panelRigidbody.position + move * Time.fixedDeltaTime;
        panelRigidbody.MovePosition(newPosition);

        //Reward Logic
        Vector3 toPanel = uiPanel.position - playerHead.position;
        float angleToPanel = Vector3.Angle(playerHead.forward, toPanel);

        if (isInPuzzleWallZone)
        {
            if (angleToPanel > 45f)
            {
                AddReward(0.05f);
                Debug.Log("[REWARD] Panel is out of FOV during puzzle wall task (+0.05)");
            }
            else
            {
                AddReward(-0.05f);
                Debug.Log("[PENALTY] Panel is in FOV during puzzle wall task (-0.05)");
            }
        }
        if (keycardPickedUp)
        {
            if (angleToPanel < 45f)
            {
                AddReward(0.05f);
                Debug.Log("[REWARD] Keycard held - panel in FOV (+0.05)");
            }
            else
            {
                AddReward(-0.05f);
                Debug.Log("[PENALTY] Keycard held - panel NOT in FOV (-0.05)");
            }
        }
    }


    //Heuristic control for debugging and testing reward functionallity before training
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        continuousActions[1] = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
        continuousActions[2] = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
    }

    //Tells the Agent to update every frame
    void Update()
    {
        RequestDecision();
    }

    //Helper methods for gamestate
    public void SetInPuzzleWallZone(bool value)
    {
        isInPuzzleWallZone = value;
    }

    public void OnKeycardPickedUp()
    {
        keycardPickedUp = true;
        Debug.Log("[GAMEPLAY] Keycard pickup state set to TRUE");
    }
}
