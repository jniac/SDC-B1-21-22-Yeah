using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerSpawnPoint : MonoBehaviour
{
    public const float debugDistanceMax = 3f;
    public static string debugInfo = "Les SpawnPoint de type \"Debug\" ne peuvent pas être activés en cours de jeu."
        + $"\n\nIls sont activés au démarrage de la session si le player (instance avec tag \"Player\") se trouve à moins de {debugDistanceMax} unité.";

    public enum Type
    {
        Normal,
        Debug,
    }

    public Type type = Type.Normal;

    public float ReachedTime { get; private set; } = -1;

    [SerializeField, HideInInspector]
    bool hasFocus = false;

    void Focus()
    {
        ReachedTime = Time.time;
        PlayerSpawnPointManager.Instance.Reach(this);
    }

    IEnumerator Start()
    {
        PlayerSpawnPointManager.Instance.Register(this);

        if (hasFocus)
            Focus();

        yield return new WaitForSeconds(0.1f);

        if (type == Type.Debug)
            DebugStart();
    }

    void DebugStart()
    {
        if (TryGetComponent<MeshRenderer>(out var mr1))
            mr1.enabled = false;

        foreach (var mr2 in GetComponentsInChildren<MeshRenderer>())
            mr2.enabled = false;

        hasFocus = GameObject.FindGameObjectsWithTag("Player")
            .Any(go => (go.transform.position - transform.position).sqrMagnitude < debugDistanceMax * debugDistanceMax);

        if (hasFocus)
            Focus();
    }

    void OnTriggerEnter(Collider other)
    {
        if (type == Type.Normal)
        {
            if (other.attachedRigidbody.gameObject.tag == "Player")
                Focus();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, .4f, 1f);
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 0.25f);
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerSpawnPoint))]
    class MyEditor : Editor
    {
        PlayerSpawnPoint Target => target as PlayerSpawnPoint;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool isDebug = Target.type == Type.Debug;

            if (isDebug == false)
            {
                GUI.enabled = false;
                EditorGUILayout.FloatField("Reached Time", Target.ReachedTime);
                EditorGUILayout.Toggle("Has Focus", Target.hasFocus);
                GUI.enabled = true;

                if (GUILayout.Button("Take \"Focus On Start\""))
                {
                    foreach (var spawnPoint in FindObjectsOfType<PlayerSpawnPoint>())
                        spawnPoint.hasFocus = false;
                    Target.hasFocus = true;
                    EditorUtility.SetDirty(target);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(debugInfo, MessageType.Info);
            }

            if (GUILayout.Button("TP Player"))
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = Target.transform.position;
                    EditorUtility.SetDirty(player);
                    VirtualCameraSwitcher.UpdatePriority(force: true);
                }
            }

            if (GUILayout.Button("TP Player Back"))
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = FindObjectsOfType<PlayerSpawnPoint>()
                        .OrderBy(spawnPoint => spawnPoint.hasFocus ? -1 : 1)
                        .First().transform.position;
                    EditorUtility.SetDirty(player);
                    VirtualCameraSwitcher.UpdatePriority(force: true);
                }
            }
        }
    }
#endif
}
