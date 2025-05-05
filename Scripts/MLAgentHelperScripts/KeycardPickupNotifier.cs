using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeycardPickupNotifier : MonoBehaviour
{
    public UIPanelAgent_GameplayContext uiAgent;

    //Tells the PanelAgent when the player is holding the Keycard object
    void OnEnable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnGrabbed);
        }
    }

    void OnDisable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (uiAgent != null)
        {
            uiAgent.OnKeycardPickedUp();
            Debug.Log("Keycard picked up");
        }
    }
}
