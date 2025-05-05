using UnityEngine;
using System.Collections;

public class KeyCardDetector : MonoBehaviour
{
    public GameObject uiText;
    private Vector3 slotEntryPosition;

    // NEW: Add a reference to the UIPanelAgent
    public UIPanelAgent panelAgent;

    void Start()
    {
        slotEntryPosition = transform.position + new Vector3(0, 0, -0.05f);
    }

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
