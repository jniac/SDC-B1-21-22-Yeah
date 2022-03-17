using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerSpawnPoint))]
    class MyEditor : Editor {
        PlayerSpawnPoint Target => target as PlayerSpawnPoint;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("TP Player"))
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    player.transform.position = Target.transform.position;
            }
        }
    }
#endif
}
