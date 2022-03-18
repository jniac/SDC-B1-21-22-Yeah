using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerSpawnPoint : MonoBehaviour
{
    public float reachedTime = -1;

    void Start()
    {
        PlayerSpawnPointManager.instance.Reach(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.tag == "Player")
        {
            reachedTime = Time.time;
            PlayerSpawnPointManager.instance.Focus(this);
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
