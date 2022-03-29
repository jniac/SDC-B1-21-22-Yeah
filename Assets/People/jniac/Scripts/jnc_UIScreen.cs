using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_UIScreen : MonoBehaviour
{
    void OnEnable()
    {
        BaseLevelManager.Instance.Pause();
        Cursor.visible = true;
    }

    void OnDisable() 
    {
        BaseLevelManager.Instance.Resume();
        Cursor.visible = false;
    }

    public void EnterScreen()
    {
        gameObject.SetActive(true);
    }

    public void ExitScreen()
    {
        gameObject.SetActive(false);
    }
}
