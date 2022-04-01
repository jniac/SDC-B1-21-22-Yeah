using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class jnc_UIScreenManager : MonoBehaviour
{
    List<GameObject> screens = new List<GameObject>();
    GameObject currentScreen = null;

    void InitScreens()
    {
        foreach (Transform child in transform) 
        {
            if (child.tag == "Screen")
            {
                var screen = child.gameObject;
                screens.Add(screen);
                screen.SetActive(false);
            }
        }
    }

    void Start()
    {
        InitScreens();
        EnterScreen("StartScreen");
    }

    public void EnterScreen(string screenName)
    {
        currentScreen = screens.FirstOrDefault(screen => screen.name == screenName);

        if (currentScreen == null)
        {
            Debug.LogWarning($"Il n'existe pas d'écran \"{screenName}\"");
            return;
        }

        foreach (var screen in screens)
            screen.SetActive(screen == currentScreen);
        
        BaseLevelManager.Instance.Pause();
        Cursor.visible = true;
    }

    public void ExitCurrentScreen()
    {
        currentScreen?.SetActive(false);
        currentScreen = null;

        BaseLevelManager.Instance.Resume();
        Cursor.visible = true;
    }
}
