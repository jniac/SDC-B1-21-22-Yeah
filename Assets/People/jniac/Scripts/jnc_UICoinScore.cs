using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class jnc_UICoinScore : MonoBehaviour
{
    jnc_Coin[] normals;
    jnc_Coin[] purples;

    TextMeshProUGUI timerText;
    TextMeshProUGUI normalText;
    TextMeshProUGUI purpleText;

    void Start()
    {
        timerText = transform.Find("Timer").GetComponent<TextMeshProUGUI>();
        normalText = transform.Find("CoinNormal").GetComponent<TextMeshProUGUI>();
        purpleText = transform.Find("CoinPurple").GetComponent<TextMeshProUGUI>();

        var all = FindObjectsOfType<jnc_Coin>();
        normals = all.Where(item => item.type == jnc_Coin.CoinType.Normal).ToArray();
        purples = all.Where(item => item.type == jnc_Coin.CoinType.Purple).ToArray();
    }

    void Update()
    {
        var ss = Mathf.Floor(Time.time % 60).ToString().PadLeft(2, '0');
        var mm = Mathf.Floor(Time.time / 60).ToString().PadLeft(2, '0');
        timerText.text = $"{mm}:{ss}";

        int normal = normals.Where(item => item == null).Count();
        int purple = purples.Where(item => item == null).Count();

        normalText.text = $"normal: {normal}/{normals.Length}";
        purpleText.text = $"purple: {purple}/{purples.Length}";
    }
}
