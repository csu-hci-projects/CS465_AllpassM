using UnityEngine;

public class PanelTrigger : MonoBehaviour
{
    private PanelOpener panelOpener;

    //If the "Special block" on the puzzle wall touches the correct edge, this calls the panel to be opened
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
