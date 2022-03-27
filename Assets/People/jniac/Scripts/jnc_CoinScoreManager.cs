using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class jnc_CoinScoreManager : MonoBehaviour
{
    jnc_Coin[] normals;
    jnc_Coin[] purples;

    public UnityEvent Win = new UnityEvent();

    void Start()
    {
        var all = FindObjectsOfType<jnc_Coin>();
        normals = all.Where(item => item.type == jnc_Coin.CoinType.Normal).ToArray();
        purples = all.Where(item => item.type == jnc_Coin.CoinType.Purple).ToArray();
    }

    void Update()
    {
        int normal = normals.Where(item => item == null).Count();
        int purple = purples.Where(item => item == null).Count();

        if (normal == 1)
        {
            Time.timeScale = 0;
            Win.Invoke();
        }
    }

}
