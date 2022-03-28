using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class jnc_LevelManager : MonoBehaviour
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

    bool debug = true;
    void Update()
    {
        int normalFound = normals.Where(item => item == null).Count();
        int purpleFound = purples.Where(item => item == null).Count();
        int totalFound = normalFound + purpleFound;

        int total = normals.Length + purples.Length;

        // if (debug && totalFound > 0)
        if (totalFound == total)
        {
            Win.Invoke();
        }
    }
}
