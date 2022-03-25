using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class jnc_CoinScore : MonoBehaviour
{
    jnc_Coin[] normals;
    jnc_Coin[] purples;
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        var all = FindObjectsOfType<jnc_Coin>();
        normals = all.Where(item => item.type == jnc_Coin.CoinType.Normal).ToArray();
        purples = all.Where(item => item.type == jnc_Coin.CoinType.Purple).ToArray();
    }

    void Update()
    {
        int normal = normals.Where(item => item == null).Count();
        int purple = purples.Where(item => item == null).Count();

        text.text = $"normal: {normal} / {normals.Length}"
            + $"\npurple: {purple} / {purples.Length}";
    }
}
