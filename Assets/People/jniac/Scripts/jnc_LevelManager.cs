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

    public jnc_Coin[] normals;
    public jnc_Coin[] purples;

    float sessionTime = 0;

    void OnEnable()
    {
        Instance = this;

        (_, normals, purples) = jnc_Coin.GetAllCoins();
    }

    void SessionUpdate()
    {
        sessionTime += Time.deltaTime;

        int normalFound = normals.Where(item => item == null).Count();
        int purpleFound = purples.Where(item => item == null).Count();
        int totalFound = normalFound + purpleFound;

        int total = normals.Length + purples.Length;

        // if (totalFound > 0) // debug
        if (totalFound == total)
        {
            hasWon = true;
            winTime = Time.time;
            Win.Invoke();
        }
    }

    void Update()
    {
        if (hasWon == false)
        {
            SessionUpdate();
        }
    }

    public float GetSessionTime()
    {
        return sessionTime;
    }
}
