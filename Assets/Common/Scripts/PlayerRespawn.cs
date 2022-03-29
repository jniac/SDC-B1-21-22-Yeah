using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PlayerRespawn : MonoBehaviour
{
    public GameObject prefabToRespawn;

    void OnDestroy()
    {
        if (Application.isPlaying)
            PlayerSpawnPointManager.Instance.Respawn();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerRespawn))]
    class MyEditor : Editor
    {
        PlayerRespawn Target => target as PlayerRespawn;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var manager = FindObjectOfType<PlayerSpawnPointManager>();
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(Target.gameObject);

            if (manager.playerPrefabToSpawn != prefab)
                EditorGUILayout.HelpBox("Attention ! Ce prefab n'est pas associé à \"PlayerSpawnPointManager\"", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("\"PlayerSpawnPointManager\" est en place.", MessageType.None);

            if (GUILayout.Button("Update \"Prefab To Respawn\""))
            {
                manager.playerPrefabToSpawn = prefab;
                EditorUtility.SetDirty(manager);
            }
        }
    }
#endif

}
