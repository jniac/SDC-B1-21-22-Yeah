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
    public UnityEvent Pause = new UnityEvent();

    jnc_Coin[] all, normals, purples;

    float sessionTime = 0;

    void OnEnable()
    {
        Instance = this;

        (all, normals, purples) = jnc_Coin.GetAllCoins();
    }

    void SessionUpdate()
    {
        sessionTime += Time.deltaTime;

        int normalFound = normals.Where(item => item == null).Count();
        int purpleFound = purples.Where(item => item == null).Count();
        int totalFound = normalFound + purpleFound;

        // if (totalFound > 1) // debug
        if (totalFound == all.Length)
        {
            hasWon = true;
            winTime = Time.time;
            Win.Invoke();
        }
    }

    public (string min, string sec, string ms) GetSessionTimeStrings()
    {
        float sessionTime = jnc_LevelManager.Instance.GetSessionTime();
        string min = Mathf.Floor(sessionTime / 60).ToString().PadLeft(2, '0');
        string sec = Mathf.Floor(sessionTime % 60).ToString().PadLeft(2, '0');
        string ms = Mathf.Floor((sessionTime % 1f) * 1000f).ToString().PadLeft(3, '0');
        return (min, sec, ms);
    }

    public (int normal, int purple) GetCoinTotal()
    {
        int normal = normals.Length;
        int purple = purples.Length;
        return (normal, purple);
    }

    public (int normal, int purple) GetCoinCount()
    {
        int normal = normals.Where(coin => coin == null).Count();
        int purple = purples.Where(coin => coin == null).Count();
        return (normal, purple);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause.Invoke();
        
        if (hasWon == false)
            SessionUpdate();
    }

    public float GetSessionTime()
    {
        return sessionTime;
    }
}
