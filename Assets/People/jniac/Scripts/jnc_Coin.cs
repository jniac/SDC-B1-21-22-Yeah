using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class jnc_Coin : MonoBehaviour
{
    public enum CoinType
    {
        Normal,
        Purple,
    }

    public static (jnc_Coin[] all, jnc_Coin[] normals, jnc_Coin[] purples) GetAllCoins() 
    {
        var all = FindObjectsOfType<jnc_Coin>();
        var normals = all.Where(item => item.type == jnc_Coin.CoinType.Normal).ToArray();
        var purples = all.Where(item => item.type == jnc_Coin.CoinType.Purple).ToArray();
        return (all, normals, purples);
    }

    public CoinType type = CoinType.Normal;

    void OnTriggerEnter(Collider other)
    {
        // The player only has the ability to collect coins.
        if (other.attachedRigidbody.gameObject.tag == "Player")
            Destroy(gameObject);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(jnc_Coin))]
    class MyEditor : Editor
    {
        jnc_Coin Target => target as jnc_Coin;
        static bool info;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            info = EditorGUILayout.BeginFoldoutHeaderGroup(info, "Info");
            if (info)
            {
                var (_, normals, purples) = jnc_Coin.GetAllCoins();
                EditorGUILayout.HelpBox($"{normals.Length} normals\n{purples.Length} purples", MessageType.None);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
#endif    
}
