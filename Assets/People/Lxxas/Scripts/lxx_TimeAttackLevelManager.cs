using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lxx_TimeAttackLevelManager : MonoBehaviour
{
    public GameObject gameoverScreen;
    public float eLapsedTime = 0f;
    public float remainingTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        gameoverScreen.SetActive(false);
        eLapsedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        eLapsedTime += Time.deltaTime;
        remainingTime -= Time.deltaTime;

        if (remainingTime < 0f)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameoverScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}