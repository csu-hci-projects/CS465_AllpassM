using UnityEngine;

public class PanelTrigger : MonoBehaviour
{
    private PanelOpener panelOpener;

    void Start()
    {
        panelOpener = FindObjectOfType<PanelOpener>(); 
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.gameObject.name);

        if (other.gameObject.CompareTag("SpecialBlock"))
        {
            Debug.Log("SpecialBlock reached the border! Opening panel...");
            panelOpener.OpenPanel();
        }
    }

}
