using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SuperCubebe
{
    [ExecuteAlways, RequireComponent(typeof(MeshFilter))]
    public class VoxelMesh : MonoBehaviour
    {
        public bool constraintChildren = true;

        long nano = -1;
        int hash = 0;

        public int ComputeHash()
        {
            int hash = constraintChildren.GetHashCode();

            unchecked
            {
                foreach (Transform child in transform)
                {
                    hash += child.position.GetHashCode();
                    hash += child.localScale.GetHashCode();
                }
            }

            return hash;
        }

        void UpdateMesh()
        {
            var st = System.Diagnostics.Stopwatch.StartNew();

            var world = VoxelWorld.FromChildren(gameObject);
            var mesh = NaiveVoxelMesher.GetMesh(world);

            st.Stop();
            // nano = st.ElapsedTicks;
            nano = ((long)1e9 * st.ElapsedTicks) / System.Diagnostics.Stopwatch.Frequency;

            GetComponent<MeshFilter>().mesh = mesh;
        }

        void UpdateChildren()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<MeshRenderer>(out var mr))
                    mr.enabled = false;

                Vector3 size = child.localScale;
                size.x = Mathf.Abs(Mathf.Round(size.x));
                size.y = Mathf.Abs(Mathf.Round(size.y));
                size.z = Mathf.Abs(Mathf.Round(size.z));

                Vector3 position = child.position;
                position += -child.localScale / 2f;
                position.x = Mathf.Round(position.x);
                position.y = Mathf.Round(position.y);
                position.z = Mathf.Round(position.z);

                child.position = position + size / 2f;
                child.localScale = size;
                child.rotation = Quaternion.identity;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateMesh();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                int newHash = ComputeHash();
                bool dirty = newHash != hash;

                if (dirty)
                {
                    hash = newHash;
                    UpdateMesh();

                    if (constraintChildren)
                        UpdateChildren();
                }
            }
#endif
        }

        void OnGUI()
        {
            // Debug.Log(Event.current);
        }

        Ray ray;
        RaycastHit hit;
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 300f);
            Gizmos.DrawSphere(hit.point, 0.25f);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(VoxelMesh))]
        class MyEditor : Editor
        {
            VoxelMesh Target => target as VoxelMesh;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                EditorGUILayout.HelpBox($"Mesh: {(float)Target.nano / 1e6:F3}ms", MessageType.None);
            }

            bool MouseRaycast(out Collider collider)
            {
                collider = null;
                float distance = float.PositiveInfinity;

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                foreach (var current in Target.GetComponentsInChildren<Collider>())
                {
                    if (current.Raycast(ray, out var hit, float.PositiveInfinity))
                    {
                        if (hit.distance < distance)
                        {
                            collider = hit.collider;
                            distance = hit.distance;
                        }
                    }
                }

                return collider != null;
            }

            void OnSceneGUI()
            {
                // ControlID ?
                // Looks like there are some new things to learn...
                // https://docs.unity3d.com/ScriptReference/GUIUtility.GetControlID.html
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                if (Event.current.type == EventType.MouseUp)
                {
                    if (MouseRaycast(out var collider))
                    {
                        Selection.activeObject = collider.gameObject;
                        Event.current.Use();
                    }
                }
            }
        }
#endif
    }
}
