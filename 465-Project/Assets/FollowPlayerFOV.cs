using UnityEngine;

public class FollowPlayerFOV : MonoBehaviour
{
    public Transform playerHead;  
    public Vector3 offset = new Vector3(0, 0, 1.0f); 

    void Update()
    {
        if (playerHead == null) return;

        Vector3 desiredPosition = playerHead.position
                                  + playerHead.forward * offset.z
                                  + playerHead.right * offset.x
                                  + playerHead.up * offset.y;

        transform.position = desiredPosition;

        transform.rotation = Quaternion.LookRotation(transform.position - playerHead.position);
    }
}
