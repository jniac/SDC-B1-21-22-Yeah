using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    void OnDestroy() 
    {
        PlayerSpawnPointManager.instance.Respawn();
    }
}
