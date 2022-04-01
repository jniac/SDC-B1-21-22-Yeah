using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class EditingBlockSnapping : MonoBehaviour
{
    public Snapping.SnapStep snapStep = Snapping.SnapStep.Unit;
    public bool drawGizmos = false;
    public Color gizmosColor = Color.yellow;

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false && Snapping.enabled)
            Snapping.ApplySnapXYZ(transform, snapStep);
#endif
    }

    void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.color = gizmosColor;
            GizmosUtils.WithAlpha(0.25f, () => 
                Gizmos.DrawCube(transform.position, transform.localScale));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EditingBlockSnapping))]
    class MyEditor : Editor
    {
        EditingBlockSnapping Target => target as EditingBlockSnapping;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Snapping.enabled = EditorGUILayout.Toggle("Snapping Enabled", Snapping.enabled);
        }
    }
#endif
}
