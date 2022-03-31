using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDebugSpawnPointAtStart : MonoBehaviour
{
    void Start()
    {
        var go = new GameObject("Debug Spawn Point");
        var spawnPoint = go.AddComponent<PlayerSpawnPoint>();
        spawnPoint.type = PlayerSpawnPoint.Type.Debug;
        spawnPoint.transform.position = transform.position;
        PlayerSpawnPointManager.Instance?.Reach(spawnPoint);     
    }
}
