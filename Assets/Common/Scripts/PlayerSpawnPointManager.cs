using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnPointManager : MonoBehaviour
{
    public static PlayerSpawnPointManager Instance { get; private set; }

    public GameObject playerPrefabToSpawn;
    public float respawnWaitTime = 1f;

    public List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();
    public List<PlayerSpawnPoint> reachedSpawnPoints = new List<PlayerSpawnPoint>();
    public PlayerSpawnPoint focusedPoint = null;

    bool isQuitting = false;

    void OnEnable()
    {
        PlayerSpawnPointManager.Instance = this;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void Register(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }

    public void Reach(PlayerSpawnPoint spawnPoint)
    {
        reachedSpawnPoints.Add(spawnPoint);
        
        focusedPoint?.BroadcastMessage("SpawnPointFocusExit", SendMessageOptions.DontRequireReceiver);
        focusedPoint = spawnPoint;
        focusedPoint.BroadcastMessage("SpawnPointFocusEnter", SendMessageOptions.DontRequireReceiver);
    }

    public void Respawn()
    {
        if (isQuitting == false && this != null)
            StartCoroutine(ThenRespawnCoroutine());
    }

    IEnumerator ThenRespawnCoroutine()
    {
        if (focusedPoint == null)
        {
            Debug.LogWarning("No activated spawn point. Cannot respawn target.");
            yield break;
        }

        yield return new WaitForSeconds(respawnWaitTime);
        Instantiate(playerPrefabToSpawn, focusedPoint.transform.position + Vector3.up, focusedPoint.transform.rotation);
    }

}
