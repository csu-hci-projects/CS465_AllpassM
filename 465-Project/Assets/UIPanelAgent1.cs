using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class UIPanelAgent1 : Agent
{
    [Header("References")]
    public Transform playerHead;
    public Transform leftController;
    public Transform rightController;
    public Transform uiPanel;
    public XRGrabInteractable keycardInteractable;
    public Collider roomBoundsCollider;
    public Collider garageBoundsCollider;
    private float maxProximityDistance = 1.5f;
    private Rigidbody panelRigidbody;


    private bool isInPuzzleWallZone = false;
    private bool isKeycardInserted = false;


    // === Ideal Offsets and Rotations for Rules ===
    private Vector3 idealOffset_R1 = new Vector3(0.03f, -0.12f, 0.93f); 
    private Vector3 idealOffset_R2 = new Vector3(0.57f, -0.17f, 1.13f); 
    private Vector3 idealOffset_R3 = new Vector3(0.87f, -0.13f, 0.71f); 
    private Vector3 idealOffset_R4 = new Vector3(2.85f, -0.30f, 1.11f); 
    private Vector3 idealOffset_R7 = new Vector3(0.03f, -0.12f, 0.93f); 

    private float maxDistance = 0.25f;
    private float maxAngle = 25f;

    private float maxAllowedDistance_R4 = 0.5f;
    private float maxAllowedAngle_R4 = 40f;

    // === Rule Flags ===
    private bool keycardPickedUp = false;
    private bool keycardInserted = false;
    private bool codeCorrect = false;

    public override void OnEpisodeBegin() { }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (playerHead == null || leftController == null || rightController == null || uiPanel == null)
        {
            Debug.LogError("[CollectObservations] One or more required transforms are NULL!");
            return;
        }

        sensor.AddObservation(playerHead.position);
        sensor.AddObservation(playerHead.forward);
        sensor.AddObservation(leftController.position);
        sensor.AddObservation(rightController.position);
        sensor.AddObservation(uiPanel.position);
    }

    public void OnKeycardInserted()
    {
        Debug.Log("[DEBUG] Keycard inserted!");
        isKeycardInserted = true;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);
        float rotationY = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);

        Vector3 move = new Vector3(moveX, moveY, moveZ) * 2f;
        Vector3 newPosition = panelRigidbody.position + move * Time.fixedDeltaTime;

        // Physics-respecting movement
        panelRigidbody.MovePosition(newPosition);

        // Smooth Y-axis rotation using angular velocity
        panelRigidbody.angularVelocity = Vector3.up * rotationY * 100f * Mathf.Deg2Rad;

        AddReward(-0.001f); // step penalty

        // FOV reward logic (angle only)
        Vector3 toPanel = uiPanel.position - playerHead.position;
        float distanceToPanel = toPanel.magnitude;
        toPanel.Normalize();

        float angleToPanel = Vector3.Angle(playerHead.forward, toPanel);

        if (angleToPanel < 30f)
        {
            AddReward(0.01f);
            Debug.Log("[REWARD] Panel is in FOV.");
        }
        else
        {
            AddReward(-0.005f);
            Debug.Log("[PENALTY] Panel is outside FOV.");
        }

        // Separate proximity reward
        if (distanceToPanel < 2.0f)
        {
            AddReward(0.005f);
            Debug.Log("[REWARD] Panel is near the player.");
        }

        Debug.Log($"[DEBUG] Angle: {angleToPanel:F2}, Distance: {distanceToPanel:F2}");

        // === Rule-based position rewards ===
        if (IsR1())
        {
            EvaluatePosition(idealOffset_R1, "R1");
        }
        else if (IsR2())
        {
            EvaluatePosition(idealOffset_R2, "R2");
        }
        else if (IsR3())
        {
            EvaluatePosition(idealOffset_R3, "R3");
        }
        else if (IsR4())
        {
            float dist = Vector3.Distance(uiPanel.position, playerHead.position);
            float angle = Quaternion.Angle(uiPanel.rotation, playerHead.rotation);

            if (dist > maxAllowedDistance_R4 || angle > maxAllowedAngle_R4)
            {
                AddReward(-0.5f);
                Debug.Log($"[PENALTY] R4 violated | Distance={dist:F2}, Angle={angle:F2}");
            }
            else
            {
                AddReward(0.5f);
                Debug.Log("[REWARD] R4 correct placement.");
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[COLLISION] Collided with: {collision.gameObject.name}");
    }


    private void EvaluatePosition(Vector3 idealOffset, string ruleName)
    {
        Vector3 targetPos = playerHead.position +
                            playerHead.right * idealOffset.x +
                            playerHead.up * idealOffset.y +
                            playerHead.forward * idealOffset.z;

        float dist = Vector3.Distance(uiPanel.position, targetPos);
        float angle = Quaternion.Angle(uiPanel.rotation, playerHead.rotation);

        if (dist < maxDistance && angle < maxAngle)
        {
            AddReward(1.0f);
            Debug.Log($"[REWARD] {ruleName} position success");
        }
        else
        {
            AddReward(-0.5f);
            Debug.Log($"[PENALTY] {ruleName} misplaced | Distance={dist:F2}, Angle={angle:F2}");
        }
    }

    // === Rule Detection ===
    private bool IsR1()
    {
        return !PlayerIsMoving() && !GazeIsMoving();
    }

    private bool IsR2()
    {
        return !PlayerIsMoving() && GazeIsMoving();
    }

    private bool IsR3()
    {
        return PlayerIsMoving(); 
    }

    private bool IsR4()
    {
        return IsLookingAtPuzzleWall() && IsOnPuzzleFloor();
    }

    // === Utility Checks (placeholders) ===
    private bool PlayerIsMoving()
    {
        return playerHead.GetComponent<Rigidbody>().linearVelocity.magnitude > 0.05f;
    }

    private bool GazeIsMoving()
    {
        // crude check: dot product between frame-to-frame forward directions
        Vector3 currentForward = playerHead.forward;
        float change = Vector3.Angle(currentForward, lastGazeDirection);
        lastGazeDirection = currentForward;
        return change > 10f;
    }

    private Vector3 lastGazeDirection;

    private bool IsLookingAtPuzzleWall()
    {
        // TODO: Replace with actual raycast or bounds check
        return false;
    }

    private bool IsOnPuzzleFloor()
    {
        return false;
    }



    void Update()
    {

        RequestDecision();

        // Debug triggers
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnKeycardPickedUp();
            Debug.Log("Simulated keycard pickup!");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 localOffset = Quaternion.Inverse(playerHead.rotation) * (uiPanel.position - playerHead.position);
            Quaternion relativeRotation = Quaternion.Inverse(playerHead.rotation) * uiPanel.rotation;
            Debug.Log($"Local panel offset from head: {localOffset}");
            Debug.Log($"Relative panel rotation (Euler): {relativeRotation.eulerAngles}");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        // Correctly mapped:
        continuousActions[0] = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0; // Left/Right (X)
        continuousActions[1] = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0; // Up/Down (Y)
        continuousActions[2] = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0; // Forward/Back (Z)
        continuousActions[3] = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0; // Rotate
    }

    void FixedUpdate()
    {
        // Debug.Log($"[TIME] fixedDeltaTime: {Time.fixedDeltaTime:F4}");
    }


    public void OnEnteredPuzzleWallZone(bool isInside)
    {
        isInPuzzleWallZone = isInside;
        Debug.Log($"[UIPanelAgent] Puzzle wall zone state: {isInPuzzleWallZone}");
    }

    public void OnKeycardPickedUp()
    {
        keycardPickedUp = true;
    }



    public void OnCodeCorrect()
    {
        codeCorrect = true;
    }

    void Start()
    {
        lastGazeDirection = playerHead.forward;
        Debug.Log("[DEBUG] UIPanelAgent has started!");
        var behaviorParams = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
        Debug.Log($"[DEBUG] Registered Behavior Name: {behaviorParams.BehaviorName}");
        panelRigidbody = uiPanel.GetComponent<Rigidbody>();
        var bounds = garageBoundsCollider.bounds;
        Debug.Log($"[DEBUG] garage Bounds Center: {bounds.center}, Size: {bounds.size}");
        Debug.Log($"Min: {bounds.min}, Max: {bounds.max}");
    }
}