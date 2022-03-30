using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour
{
    string label = "";
    float count;
    GUIStyle guiStyle;

    IEnumerator Start()
    {
        
        guiStyle = new GUIStyle {
            fontSize = 20,
            alignment = TextAnchor.MiddleRight,
        };

        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                count = (1 / Time.deltaTime);
                label = $"{Mathf.Round(count)} fps {Screen.width}x{Screen.height}";
            }
            else
            {
                label = "Pause";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnGUI()
    {
        int width = 100;
        GUI.Label(new Rect(Screen.width - 25 - width, 20, width, 25), label, guiStyle);
    }
}