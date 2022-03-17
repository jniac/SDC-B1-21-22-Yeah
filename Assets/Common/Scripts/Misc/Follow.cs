using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class Follow : MonoBehaviour
{
    public Transform target;

    [Range(0, 1)]
    public float damping = 0.5f;

    public bool useRotation = true;

    Vector3 positionStart;
    Quaternion rotationStart;

    Vector3 positionOld;
    Quaternion rotationOld;

    void Start()
    {
        if (target == null)
            return;

        positionStart = target.position;
        rotationStart = target.rotation;
        positionOld = target.position;
        rotationOld = target.rotation;
    }

    void Update()
    {
        if (target == null)
            return;

#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
#endif
        transform.position = Vector3.Lerp(positionOld, target.position, damping);
        transform.rotation = useRotation ? Quaternion.Slerp(rotationOld, target.rotation, damping) : rotationStart;
        
        positionOld = transform.position;
        rotationOld = transform.rotation;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Follow))]
    class MyEditor : Editor
    {
        Follow Target => target as Follow;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Follow Parent"))
                Target.target = Target.transform.parent;
        }
    }
#endif
}
