using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PuzzleBlockMover : MonoBehaviour
{
    public float minZ = 1.7f;
    public float maxZ = -1.7f;
    private XRGrabInteractable grabInteractable;
    private Vector3 initialPosition;


    //Allows you to grab the blocks on the puzzle wall
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectExited.AddListener((SelectExitEventArgs args) => OnRelease());

    }

    void Update()
    {
        if (grabInteractable.isSelected)
        {
            Vector3 newPosition = transform.position;

            //Restricts movement to Z axis
            newPosition.x = initialPosition.x;
            newPosition.y = initialPosition.y;

            newPosition.z = Mathf.Clamp(transform.position.z, maxZ, minZ);

            transform.position = newPosition;
        }
    }

    void OnRelease()
    {
        //Colliders should stop this, but failsafe clamping if it somehow gets out of bounds of the wall
        Vector3 newPosition = transform.position;
        newPosition.z = Mathf.Clamp(newPosition.z, maxZ, minZ);
        transform.position = newPosition;
    }

}
