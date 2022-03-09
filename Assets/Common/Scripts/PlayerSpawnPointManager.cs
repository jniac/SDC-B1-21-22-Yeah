using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnPointManager : MonoBehaviour
{
    public static PlayerSpawnPointManager instance;

    public GameObject playerPrefabToSpawn;
    public float respawnWaitTime = 1f;

    public List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();

    bool isQuitting = false;

    void OnEnable()
    {
        PlayerSpawnPointManager.instance = this;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public PlayerSpawnPoint GetLastActivatedSpawnPoint()
    {
        PlayerSpawnPoint result = null;
        foreach (var current in spawnPoints)
        {
            if (current.activated)
            {
                if (result == null)
                {
                    result = current;
                }
                else if (result.activationTime < current.activationTime)
                {
                    result = current;
                }
            }
        }
        return result;
    }

    public void Respawn()
    {
        if (isQuitting == false)
            StartCoroutine(ThenRespawnCoroutine());
    }

    IEnumerator ThenRespawnCoroutine()
    {
        PlayerSpawnPoint spawnPoint = GetLastActivatedSpawnPoint();

        if (spawnPoint == null)
        {
            Debug.LogWarning("No activated spawn point. Cannot respawn target.");
            yield break;
        }

        yield return new WaitForSeconds(respawnWaitTime);
        Instantiate(playerPrefabToSpawn, spawnPoint.transform.position + Vector3.up, spawnPoint.transform.rotation);
    }

}
