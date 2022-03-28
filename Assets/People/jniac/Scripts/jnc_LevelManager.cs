using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class jnc_LevelManager : MonoBehaviour
{
    public static jnc_LevelManager Instance { get; private set; }

    public bool hasWon = false;
    public float winTime = -1;
    public UnityEvent Win = new UnityEvent();

    jnc_Coin[] normals;
    jnc_Coin[] purples;

    void Start()
    {
        Instance = this;

        var all = FindObjectsOfType<jnc_Coin>();
        normals = all.Where(item => item.type == jnc_Coin.CoinType.Normal).ToArray();
        purples = all.Where(item => item.type == jnc_Coin.CoinType.Purple).ToArray();

        BaseLevelManager.Instance.Pause();
    }

    void SessionUpdate()
    {
        int normalFound = normals.Where(item => item == null).Count();
        int purpleFound = purples.Where(item => item == null).Count();
        int totalFound = normalFound + purpleFound;

        int total = normals.Length + purples.Length;

        if (totalFound > 0)
        // if (totalFound == total)
        {
            hasWon = true;
            winTime = Time.time;
            Win.Invoke();
        }
    }

    void Update()
    {
        if (hasWon == false)
            SessionUpdate();
    }

    public float GetSessionTime()
    {
        return hasWon ? winTime : Time.time;
    }
}
