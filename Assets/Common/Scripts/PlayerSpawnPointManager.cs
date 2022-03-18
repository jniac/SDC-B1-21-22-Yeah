using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnPointManager : MonoBehaviour
{
    public static PlayerSpawnPointManager instance;

    public GameObject playerPrefabToSpawn;
    public float respawnWaitTime = 1f;

    public List<PlayerSpawnPoint> reachedPoints = new List<PlayerSpawnPoint>();
    public PlayerSpawnPoint focusedPoint = null;

    bool isQuitting = false;

    void OnEnable()
    {
        PlayerSpawnPointManager.instance = this;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void Reach(PlayerSpawnPoint spawnPoint)
    {
        reachedPoints.Add(spawnPoint);
    }

    public void Focus(PlayerSpawnPoint spawnPoint)
    {
        focusedPoint?.BroadcastMessage("SpawnPointFocusExit");
        focusedPoint = spawnPoint;
        focusedPoint.BroadcastMessage("SpawnPointFocusEnter");
    }

    public void Respawn()
    {
        if (isQuitting == false)
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
