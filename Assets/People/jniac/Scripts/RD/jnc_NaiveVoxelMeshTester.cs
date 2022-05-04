using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class jnc_NaiveVoxelMeshTester : MonoBehaviour
{
    void Build()
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        var world = SuperCubebe.VoxelWorld.New(blocks);
        GetComponent<MeshFilter>().mesh = SuperCubebe.NaiveVoxelMesher.GetMesh(world);

        foreach (var block in blocks)
            block.GetComponent<MeshRenderer>().enabled = false;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(jnc_NaiveVoxelMeshTester))]
    class MyEditor : Editor
    {
        jnc_NaiveVoxelMeshTester Target => target as jnc_NaiveVoxelMeshTester;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Build"))
                Target.Build();
        }
    }
#endif    
}
