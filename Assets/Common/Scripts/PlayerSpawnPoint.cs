using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerSpawnPoint : MonoBehaviour
{
    public float reachedTime = -1;

    public bool focusOnStart = false;

    void Focus()
    {
        reachedTime = Time.time;
        PlayerSpawnPointManager.instance.Focus(this);
    }

    void Start()
    {
        PlayerSpawnPointManager.instance.Reach(this);

        if (focusOnStart)
            Focus();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.tag == "Player")
            Focus();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerSpawnPoint))]
    class MyEditor : Editor
    {
        PlayerSpawnPoint Target => target as PlayerSpawnPoint;
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;

            if (GUILayout.Button("Take \"Focus On Start\""))
            {
                foreach (var spawnPoint in FindObjectsOfType<PlayerSpawnPoint>())
                    spawnPoint.focusOnStart = false;
                Target.focusOnStart = true;
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("TP Player"))
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) 
                {
                    player.transform.position = Target.transform.position;
                    EditorUtility.SetDirty(player);
                }
            }

            if (GUILayout.Button("TP Player Back"))
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) 
                {
                    player.transform.position = FindObjectsOfType<PlayerSpawnPoint>()
                        .OrderBy(spawnPoint => spawnPoint.focusOnStart ? -1 : 1)
                        .First().transform.position;
                    EditorUtility.SetDirty(player);
                }
            }
        }
    }
#endif
}
