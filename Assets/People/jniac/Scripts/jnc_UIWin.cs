using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class jnc_UIWin : MonoBehaviour
{
    Transform win;
    TextMeshProUGUI timerText;

    void Start()
    {
        win = transform.Find("WinScreen");
        timerText = win.FindDeepChild("Timer").GetComponent<TextMeshProUGUI>();
        
        win.gameObject.SetActive(false);
    }

    public void EnterWinScreen()
    {
        var ss = Mathf.Floor(Time.time % 60).ToString().PadLeft(2, '0');
        var mm = Mathf.Floor(Time.time / 60).ToString().PadLeft(2, '0');
        timerText.text = $"{mm}:{ss}";
        
        win.gameObject.SetActive(true);
    }
}
