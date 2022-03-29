using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class jnc_UIWinScreen : MonoBehaviour
{
    void Update()
    {
        float time = jnc_LevelManager.Instance.GetSessionTime();
        var ss = Mathf.Floor(time % 60f).ToString().PadLeft(2, '0');
        var mm = Mathf.Floor(time / 60f).ToString().PadLeft(2, '0');
        var ms = Mathf.Floor((time % 1f) * 1000f).ToString().PadLeft(3, '0');
        
        var timerText = transform.FindDeepChild("Timer").GetComponent<TextMeshProUGUI>();
        timerText.text = $"{mm}:{ss}.{ms}";
    }
}
