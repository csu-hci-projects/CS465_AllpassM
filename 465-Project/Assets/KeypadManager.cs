using UnityEngine;
using TMPro;

public class KeypadManager : MonoBehaviour
{
    public TextMeshProUGUI keypadDisplay; 
    private string enteredCode = "";
    public string correctCode = "9999";


    public void ButtonPressed(string value)
    {
        if (value == "DEL")
        {
            if (enteredCode.Length > 0)
                enteredCode = enteredCode.Substring(0, enteredCode.Length - 1);
        }
        else
        {
            enteredCode += value;
        }

        keypadDisplay.text = enteredCode;

        if (enteredCode == correctCode)
        {
            Victory();
        }
    }

    void Victory()
    {
        keypadDisplay.text = "You win!";
        Debug.Log("Puzzle Solved!");

        var agent = FindObjectOfType<UIPanelAgent>();
        if (agent != null)
        {
            agent.OnCodeCorrect();
        }
    }


}
