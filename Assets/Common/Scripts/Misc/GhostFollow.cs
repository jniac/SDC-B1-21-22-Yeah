using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GhostFollow : MonoBehaviour
{
    public Transform target;

    [Range(0, 1)]
    public float damping = 0.5f;

    public Vector3 positionOffset = Vector3.zero;

    public bool useRotation = true;

    Vector3 positionStart;
    Quaternion rotationStart;

    Vector3 position;
    Quaternion rotation;

    Vector3 positionOld;
    Quaternion rotationOld;

    Vector3 TargetPosition => (target?.position ?? Vector3.zero) + positionOffset;
    Quaternion TargetRotation => target?.rotation ?? Quaternion.identity;

    void CopyTarget()
    {
        transform.position = position = positionStart = positionOld = TargetPosition;
        transform.rotation = rotation = rotationStart = rotationOld = TargetRotation;
    }

    void Compute()
    {
        if (target == null)
            return;

        position = Vector3.Lerp(positionOld, TargetPosition + positionOffset, damping);
        rotation = useRotation ? Quaternion.Slerp(rotationOld, target.rotation, damping) : rotationStart;

        positionOld = position;
        rotationOld = rotation;
    }

    void Start()
    {
        CopyTarget();
    }

    void FixedUpdate()
    {
        Compute();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
            CopyTarget();
#endif

        transform.position = position;
        transform.rotation = rotation;
    }

    void OnValidate()
    {
        CopyTarget();
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(GhostFollow))]
    class MyEditor : Editor
    {
        GhostFollow Target => target as GhostFollow;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Follow Parent"))
            {
                Target.target = Target.transform.parent;
                EditorUtility.SetDirty(target);
            }
        }
    }
#endif
}
