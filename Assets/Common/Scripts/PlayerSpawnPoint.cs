using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public bool activated = false;
    public float activationTime = -1;

    void Start()
    {
        PlayerSpawnPointManager.instance.spawnPoints.Add(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.tag == "Player")
        {
            activated = true;
            activationTime = Time.time;
        }
    }
}
