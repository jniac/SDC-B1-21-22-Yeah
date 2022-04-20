using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class jnc_UIRunTimeScore : MonoBehaviour
{
    TMPro.TextMeshProUGUI timerText, coinNormalText, coinPurpleText;

    void Start()
    {
        timerText = transform
            .Find("Timer")
            .GetComponent<TMPro.TextMeshProUGUI>();

        coinNormalText = transform
            .Find("CoinNormal")
            .GetComponent<TMPro.TextMeshProUGUI>();

        coinPurpleText = transform
            .Find("CoinPurple")
            .GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        var (min, sec, ms) = jnc_LevelManager.Instance.GetSessionTimeStrings();
        var (normalCount, purpleCount) = jnc_LevelManager.Instance.GetCoinCount();
        var (normalTotal, purpleTotal) = jnc_LevelManager.Instance.GetCoinTotal();

        timerText.text = $"{min}:{sec}";
        coinNormalText.text = $"Normal: {normalCount}/{normalTotal}";
        coinPurpleText.text = $"Normal: {purpleCount}/{purpleTotal}";
    }
}
