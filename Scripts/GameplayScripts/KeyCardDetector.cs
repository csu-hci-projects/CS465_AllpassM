using UnityEngine;
using System.Collections;

public class KeyCardDetector : MonoBehaviour
{
    public GameObject uiText;
    private Vector3 slotEntryPosition;
    public UIPanelAgent panelAgent;

    //Allows Keycard slot to recognize when the Keycard is colliding
    void Start()
    {
        slotEntryPosition = transform.position + new Vector3(0, 0, -0.05f);
    }

    //Plays animation of card sliding in after turning off ability for the player to grab it anymore
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KeyCard"))
        {
            var grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }

            other.transform.position = slotEntryPosition;
            other.transform.rotation = transform.rotation;

            StartCoroutine(SlideCardIn(other.gameObject));
        }
    }
    //Animation method, after it finishes it also changes the text to display the code
    IEnumerator SlideCardIn(GameObject keyCard)
    {
        Vector3 finalPosition = transform.position;
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (keyCard == null) yield break;

            keyCard.transform.position = Vector3.Lerp(keyCard.transform.position, finalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (keyCard != null)
        {
            keyCard.transform.position = finalPosition;

            
            if (panelAgent != null)
            {
                panelAgent.OnKeycardInserted();
                Debug.Log("[KeyCardDetector] Notified UIPanelAgent of keycard insertion.");
            }

            Destroy(keyCard); 
        }

        uiText.GetComponent<TMPro.TextMeshProUGUI>().text = "Code 9999";
    }
}
