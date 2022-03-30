using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum PlayMode
{
    Normal = 0,
    AlwaysJump = 1,
    NeverDie = 2,
    God = ~0,
}

public class PlayModeManager : MonoBehaviour
{
    public static PlayModeManager instance;

    public static bool Test(PlayMode mode) => ((instance?.playMode ?? PlayMode.Normal) & mode) != 0;

    public PlayMode playMode = PlayMode.Normal;

    void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
