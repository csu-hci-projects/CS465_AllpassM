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

            // Restrict movement to Z-axis
            newPosition.x = initialPosition.x;
            newPosition.y = initialPosition.y;

            // Clamp Z position within boundaries
            newPosition.z = Mathf.Clamp(transform.position.z, maxZ, minZ);

            transform.position = newPosition;
        }
    }

    void OnRelease()
    {
        // Snap to valid position when released
        Vector3 newPosition = transform.position;
        newPosition.z = Mathf.Clamp(newPosition.z, maxZ, minZ);
        transform.position = newPosition;
    }

}
