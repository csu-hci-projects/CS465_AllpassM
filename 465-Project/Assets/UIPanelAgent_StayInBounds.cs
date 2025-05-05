using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class UIPanelAgent_StayInBounds : Agent
{
    [Header("References")]
    public Transform uiPanel; // Panel we are controlling
    public Collider roomBoundsCollider; // Large outer box collider
    public Collider garageBoundsCollider; // Small inner box collider

    private Rigidbody panelRigidbody;

    private bool isInRoomBounds = false;
    private bool isInGarageBounds = false;

    public override void Initialize()
    {
        panelRigidbody = uiPanel.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
       

        panelRigidbody.linearVelocity = Vector3.zero;
        panelRigidbody.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Minimal observation: panel position
        sensor.AddObservation(uiPanel.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("[DEBUG] OnActionReceived triggered");

        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);

        Vector3 move = new Vector3(moveX, moveY, moveZ) * 2f; // adjust movement speed here
        Vector3 newPosition = panelRigidbody.position + move * Time.fixedDeltaTime;

        panelRigidbody.MovePosition(newPosition);

        // === Reward logic based on collision flags ===
        if (isInRoomBounds)
        {
            AddReward(0.01f); // small positive reward per step inside room
            Debug.Log("[REWARD] Panel inside room bounds (+0.01)");
        }
        else
        {
            AddReward(-0.05f); // small penalty per step outside
            Debug.Log("[PENALTY] Panel outside room bounds (-0.05)");
        }

        if (isInGarageBounds)
        {
            AddReward(-0.2f); // larger penalty for being inside garage
            Debug.Log("[PENALTY] Panel inside garage bounds (-0.2)");
        }
    }


    void Update()
    {
        RequestDecision();
    }


    public void SetInRoomBounds(bool value)
    {
        isInRoomBounds = value;
    }

    public void SetInGarageBounds(bool value)
    {
        isInGarageBounds = value;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        continuousActions[1] = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
        continuousActions[2] = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
    }
}
