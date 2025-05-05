using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class UIPanelAgent_StayInBounds : Agent
{
    //Vector observations this agent has access to
    public Transform uiPanel;
    public Collider roomBoundsCollider;
    public Collider garageBoundsCollider;
    private Rigidbody panelRigidbody;
    private bool isInRoomBounds = false;
    private bool isInGarageBounds = false;

    public override void Initialize()
    {
        panelRigidbody = uiPanel.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(uiPanel.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Allows the panel to be moved by the agent
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);

        Vector3 move = new Vector3(moveX, moveY, moveZ) * 2f;
        Vector3 newPosition = panelRigidbody.position + move * Time.fixedDeltaTime;

        panelRigidbody.MovePosition(newPosition);

        //Reward logic
        if (isInRoomBounds)
        {
            AddReward(0.01f);
            Debug.Log("[REWARD] Panel inside room bounds (+0.01)");
        }
        else
        {
            AddReward(-0.05f);
            Debug.Log("[PENALTY] Panel outside room bounds (-0.05)");
        }

        if (isInGarageBounds)
        {
            AddReward(-0.2f);
            Debug.Log("[PENALTY] Panel inside garage bounds (-0.2)");
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

    //Helper methods
    public void SetInRoomBounds(bool value)
    {
        isInRoomBounds = value;
    }

    public void SetInGarageBounds(bool value)
    {
        isInGarageBounds = value;
    }


    
}
