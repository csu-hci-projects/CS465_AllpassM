using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceControl : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    public GameObject uiPanel;

    void Start()
    {
        keywords.Add("move", MovePanel);

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();

        Debug.Log("Voice recognizer started. Say 'move'.");
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Heard command: " + args.text);
        if (keywords.ContainsKey(args.text))
        {
            keywords[args.text].Invoke();
        }
    }

    private void MovePanel()
    {
        Debug.Log("MovePanel triggered!");

        Transform cameraTransform = Camera.main.transform;
        Vector3 newPosition = cameraTransform.position + cameraTransform.forward * 1.5f;
        newPosition.y = cameraTransform.position.y; 

        uiPanel.transform.position = newPosition;
        uiPanel.transform.LookAt(cameraTransform);
        uiPanel.transform.Rotate(0, 180, 0); 
    }
}
