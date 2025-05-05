using UnityEngine;
using UnityEngine.InputSystem;

public class PanelOpener : MonoBehaviour
{
    public float openAngle = -45f; 
    public float speed = 2f;
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    //If called, plays animation of panel opening
    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(openAngle, 0, 0) * closedRotation;
    }

    void Update()
    {
        if (isOpening)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * speed);

            if (Quaternion.Angle(transform.rotation, openRotation) < 0.5f)
            {
                transform.rotation = openRotation;
                isOpening = false;
            }
        }

        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        isOpening = true;
    }
}
