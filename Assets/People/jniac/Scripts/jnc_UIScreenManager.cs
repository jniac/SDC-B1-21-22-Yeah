using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class jnc_UIScreenManager : MonoBehaviour
{
    public Transform start, win;

    void Start()
    {
        win.gameObject.SetActive(false);
    }

    public void ExitStartScreen()
    {
        start.gameObject.SetActive(false);
    }

    public void EnterWinScreen()
    {
        float time = jnc_LevelManager.Instance.GetSessionTime();
        var ss = Mathf.Floor(time % 60f).ToString().PadLeft(2, '0');
        var mm = Mathf.Floor(time / 60f).ToString().PadLeft(2, '0');
        var ms = Mathf.Floor((time % 1f) * 1000f).ToString().PadLeft(3, '0');
        var timerText = win.FindDeepChild("Timer").GetComponent<TextMeshProUGUI>();
        timerText.text = $"{mm}:{ss}.{ms}";
        
        win.gameObject.SetActive(true);
    }
}
