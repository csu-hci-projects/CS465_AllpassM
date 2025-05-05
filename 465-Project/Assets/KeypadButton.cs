using UnityEngine;
using System.Collections;

public class KeypadButton : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isPressed = false;
    public string buttonValue;


    void Start()
    {
        originalPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.CompareTag("PlayerHand"))
        {
            StartCoroutine(PressButton());

            FindObjectOfType<KeypadManager>().ButtonPressed(buttonValue);
        }
    }



    IEnumerator PressButton()
    {
        isPressed = true;
        Vector3 pressedPosition = originalPosition + new Vector3(0, -0.02f, 0); 
        float pressSpeed = 0.05f;

        float elapsedTime = 0f;
        while (elapsedTime < pressSpeed)
        {
            transform.position = Vector3.Lerp(originalPosition, pressedPosition, elapsedTime / pressSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = pressedPosition;

        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;
        while (elapsedTime < pressSpeed)
        {
            transform.position = Vector3.Lerp(pressedPosition, originalPosition, elapsedTime / pressSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        isPressed = false;
    }
}
