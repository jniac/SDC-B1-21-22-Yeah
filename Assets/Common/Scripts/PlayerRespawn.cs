using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float waitTime = 1f;

    void OnDestroy() 
    {
        PlayerSpawnPointManager.instance.Respawn();
    }
}
