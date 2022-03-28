using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseLevelManager : MonoBehaviour
{
    public static BaseLevelManager Instance { get; private set; }



    // Some public static status.
    static bool isRunning = false;
    public static bool IsRunning { get => isRunning && Application.isPlaying; }



    // Public methods (for event binding).

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void ReloadCurrentScene()
    {
        isRunning = false;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
    }



    // Lifecycle:

    void OnEnable()
    {
        Instance = this;
    }

    void Start()
    {
        isRunning = true;
    }

    void OnApplicationQuit() 
    {
        isRunning = false;
    }

    void OnDestroy()
    {
        isRunning = false;
        Instance = null;
    }
}
