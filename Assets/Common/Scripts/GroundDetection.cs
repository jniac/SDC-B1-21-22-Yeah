using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GroundDetection : MonoBehaviour
{

    public LayerMask groundMask = ~0;
    public float maxGroundDistance = 0.5f;
    public float maxCloseGroundDistance = 1f;

    [System.NonSerialized]
    public bool onGround = false;

    [System.NonSerialized]
    public bool closeToTheGround = false;

    bool CastGround(float distance)
    {
        Vector3 origin = transform.position + Vector3.down * 0.45f;
        
        if (Physics.Raycast(origin, Vector3.down, distance, groundMask, QueryTriggerInteraction.Ignore))
            return true;

        return false;
    }

    private void FixedUpdate()
    {
        onGround = CastGround(maxGroundDistance);
        closeToTheGround = CastGround(maxCloseGroundDistance);
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.down * 0.5f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, Vector3.down * maxCloseGroundDistance);
        Gizmos.DrawSphere(origin + Vector3.down * maxGroundDistance, 0.033f);
        Gizmos.DrawSphere(origin + Vector3.down * maxCloseGroundDistance, 0.033f);
        Gizmos.DrawSphere(origin, 0.1f);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GroundDetection))]
    class MyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.enabled = false;
            EditorGUILayout.Toggle("On Ground", (target as GroundDetection).onGround);
        }
    }
#endif
}
