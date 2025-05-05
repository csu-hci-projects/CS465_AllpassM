using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class UIPanelAgent_FOV : Agent
{
    [Header("References")]
    public Transform uiPanel;
    public Transform playerHead;

    private Rigidbody panelRigidbody;

    [Tooltip("If enabled, disables agent control so the user can manually grab/move the panel.")]
    public bool allowManualGrabOverride = false;

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
        sensor.AddObservation(uiPanel.position);
        sensor.AddObservation(uiPanel.forward);
        sensor.AddObservation(playerHead.position);
        sensor.AddObservation(playerHead.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (allowManualGrabOverride)
        {
            Debug.Log("[INFO] Manual override active � skipping agent control.");
            return;
        }

        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);
        float rotateY = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);

        Vector3 move = new Vector3(moveX, moveY, moveZ) * 2f;
        Vector3 newPosition = panelRigidbody.position + move * Time.fixedDeltaTime;

        panelRigidbody.MovePosition(newPosition);
        panelRigidbody.angularVelocity = Vector3.up * rotateY * 100f * Mathf.Deg2Rad;

        AddReward(-0.001f); // step penalty

        // === FOV reward ===
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

        // === Facing the player reward ===
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

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        continuousActions[1] = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
        continuousActions[2] = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        continuousActions[3] = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
    }

    void Update()
    {
        RequestDecision();
    }
}
