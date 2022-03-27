using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class jnc_UIWin : MonoBehaviour
{
    Transform win;
    TextMeshProUGUI timerText;

    void Start()
    {
        win = transform.Find("Win");
        timerText = win.Find("Timer").GetComponent<TextMeshProUGUI>();
        FindObjectOfType<jnc_CoinScoreManager>().Win.AddListener(Win);
        win.gameObject.SetActive(false);
    }

    void Win()
    {
        var ss = Mathf.Floor(Time.time % 60).ToString().PadLeft(2, '0');
        var mm = Mathf.Floor(Time.time / 60).ToString().PadLeft(2, '0');
        timerText.text = $"{mm}:{ss}";
        win.gameObject.SetActive(true);
    }
}
