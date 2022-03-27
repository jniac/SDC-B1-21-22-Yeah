using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class jnc_UIWin : MonoBehaviour
{
    TextMeshProUGUI timerText;

    void Start()
    {
        timerText = transform.Find("Timer").GetComponent<TextMeshProUGUI>();
        FindObjectOfType<jnc_CoinScoreManager>().Win.AddListener(Win);
        transform.Find("Win").gameObject.SetActive(false);
    }

    void Win()
    {
        transform.Find("Win").gameObject.SetActive(true);
        var ss = Mathf.Floor(Time.time % 60).ToString().PadLeft(2, '0');
        var mm = Mathf.Floor(Time.time / 60).ToString().PadLeft(2, '0');
        timerText.text = $"{mm}:{ss}";
    }
}
