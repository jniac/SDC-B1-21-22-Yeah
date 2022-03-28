using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseLevelManager : MonoBehaviour
{
    static BaseLevelManager instance;

    static GameObject CreateGO()
    {
        Debug.Log("Creating LevelManager GameObject instance from script.");
        return new GameObject("LevelManager");
    }

    static BaseLevelManager CreateLevelManager(GameObject go)
    {
        Debug.Log("Creating LevelManager Component instance from script.");
        return go.AddComponent<BaseLevelManager>();
    }

    static BaseLevelManager InitInstance()
    {
        var go = GameObject.Find("LevelManager") ?? CreateGO();
        instance = go.GetComponent<BaseLevelManager>() ?? CreateLevelManager(go);
        return instance;
    }

    public static BaseLevelManager Instance { get => instance ?? InitInstance(); }

    // Hm... this is a way to enforce the existence of an LevelManager instance.
    // BUT those kind of hack has a lot of drawbacks (eg: does not work well with hot reload).
    // Not sure of that solution.
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        InitInstance();
    }



    // Some public static status.
    static bool isPlaying = false;
    public static bool IsPlaying { get => isPlaying && Application.isPlaying; }



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
        isPlaying = false;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
    }



    // Lifecycle:

    void Start()
    {
        Resume();
        isPlaying = true;
    }

    void OnApplicationQuit() 
    {
        isPlaying = false;
    }

    void OnDestroy()
    {
        isPlaying = false;
    }
}
