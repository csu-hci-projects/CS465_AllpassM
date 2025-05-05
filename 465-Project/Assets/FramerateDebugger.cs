using UnityEngine;

public class FramerateDebugger : MonoBehaviour
{
    float timer = 0f;

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer > 1f)
        {
            
            timer = 0f;
        }
    }
}
