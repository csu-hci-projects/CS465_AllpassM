using UnityEngine;

public class PlayerSpeedDebugger : MonoBehaviour
{
    private Vector3 lastPosition;
    public string label = "Object";
    //Calculates player's speed based on position over time
    void Start()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        float speed = distance / Time.fixedDeltaTime;
        lastPosition = transform.position;

        Debug.Log(label + " Speed: " + speed);
    }
}
