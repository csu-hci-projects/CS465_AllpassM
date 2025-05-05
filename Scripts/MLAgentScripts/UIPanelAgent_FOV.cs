using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class UIPanelAgent_FOV : Agent
{
    //Vector Observations this agent has access to
    public Transform uiPanel;
    public Transform playerHead;
    private Rigidbody panelRigidbody;

    public override void Initialize()
    {
        panelRigidbody = uiPanel.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(uiPanel.position);
        sensor.AddObservation(uiPanel.forward);
        sensor.AddObservation(playerHead.position);
        sensor.AddObservation(playerHead.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Allows the panel to be moved by the agent
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);
        float rotateY = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);

        Vector3 move = new Vector3(moveX, moveY, moveZ) * 2f;
        Vector3 newPosition = panelRigidbody.position + move * Time.fixedDeltaTime;

        panelRigidbody.MovePosition(newPosition);
        panelRigidbody.angularVelocity = Vector3.up * rotateY * 100f * Mathf.Deg2Rad;

        //step penalty
        AddReward(-0.001f); 

        //Reward Logic
        Vector3 toPanel = uiPanel.position - playerHead.position;
        float distanceToPanel = toPanel.magnitude;
        toPanel.Normalize();

        float angleToPanel = Vector3.Angle(playerHead.forward, toPanel);

        if (angleToPanel < 60f)
        {
            AddReward(0.02f);
            Debug.Log($"[REWARD] Panel in FOV (angle={angleToPanel:F2})");
        }
        else
        {
            AddReward(-0.01f);
            Debug.Log($"[PENALTY] Panel out of FOV (angle={angleToPanel:F2})");
        }

        float facingAngle = Vector3.Angle(uiPanel.forward, -toPanel);
        if (facingAngle < 20f)
        {
            AddReward(0.03f);
            Debug.Log($"[REWARD] Panel facing player (facingAngle={facingAngle:F2})");
        }
        else
        {
            Debug.Log($"[INFO] Panel not facing player (facingAngle={facingAngle:F2})");
        }
    }

    //Heuristic control for debugging and testing reward functionallity before training
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        continuousActions[1] = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
        continuousActions[2] = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        continuousActions[3] = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
    }

    //Tells the Agent to update every frame
    void Update()
    {
        RequestDecision();
    }
}
