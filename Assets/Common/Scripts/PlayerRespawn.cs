using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public GameObject[] onDestroyParticles;

    void OnDestroy() 
    {
        Boom.FromPoint(transform.position, 20, onDestroyParticles);
        
        PlayerSpawnPointManager.instance.Respawn();
    }
}
