using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode, RequireComponent(typeof(EditingBlockSnapping))]
public class VirtualCameraSwitcher : MonoBehaviour
{
    static List<VirtualCameraSwitcher> instances = new List<VirtualCameraSwitcher>();

    static CinemachineVirtualCamera[] vcams;
    static CinemachineVirtualCamera currentVcam;
    static CinemachineVirtualCamera defaultVcam;
    static Transform follow;
    static void FindVcams(bool force = false)
    {
        if (vcams != null && force == false)
            return;

        vcams = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();

        // The default vcam is the first vcam that is not controlled 
        // by a VirtualCameraSwitcher instance.
        defaultVcam = vcams
            .Where(vcam => instances.All(switcher => switcher.vcam != vcam))
            .FirstOrDefault();
    }

    static int updatePriorityFrame = -1;
    public static void UpdatePriority(bool force = false)
    {
        if (force == false && Time.frameCount % 10 != 0)
            return;

        bool alreadyUpdated = updatePriorityFrame == Time.frameCount;
        if (force == false && alreadyUpdated)
            return;

        updatePriorityFrame = Time.frameCount;

        FindVcams(force);

        follow = instances
            .Select(item => item.vcam?.Follow)
            .Where(item => item != null)
            .FirstOrDefault();

        if (follow == null)
            return;

        // Compute the maximum priority for each vcam.
        var vcamPriority = new Dictionary<CinemachineVirtualCamera, int>();
        foreach (var instance in instances)
        {
            bool overlaps = instance.Overlaps(follow.position);
            int priority = overlaps ?
                instance.onEnterPriority :
                defaultVcam ? defaultVcam.Priority - 1 : 10;

            if (vcamPriority.ContainsKey(instance.vcam))
            {
                var current = vcamPriority[instance.vcam];
                vcamPriority[instance.vcam] = Mathf.Max(current, priority);
            }
            else
            {
                vcamPriority.Add(instance.vcam, priority);
            }
        }

        // Update each vcam with its previously computed priority.
        foreach (var entry in vcamPriority)
        {
            var vcam = entry.Key;
            int priority = entry.Value;
            vcam.Priority = priority;

#if UNITY_EDITOR
            EditorUtility.SetDirty(vcam);
#endif
        }

        var newVcam = vcams.OrderBy(vcam => vcam.Priority).LastOrDefault();
        if (newVcam != currentVcam)
            Player.BroadcastAll("OnSwitchCamera", newVcam);
        currentVcam = newVcam;
    }

    public CinemachineVirtualCamera vcam;
    public int onEnterPriority = 20;
    public Vector3 safeMargin = Vector3.one * 0.5f;

    public Bounds Bounds => new Bounds(transform.position, transform.localScale + safeMargin);

    public bool Overlaps(Vector3 point) => Bounds.Contains(point);

    void OnEnable()
    {
        instances.Add(this);
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    void Update()
    {
        UpdatePriority();
    }

    void OnValidate()
    {
        var str = vcam != null ? vcam.name.Substring(vcam.name.Length - 5) : "...";
        gameObject.name = $"SwitchTo ({str}:{onEnterPriority})";
    }

    public Color gizmoColor = Color.yellow;
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        var bounds = Bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        var bounds = Bounds;
        GizmoPrimitives.WithAlpha(0.2f, () => Gizmos.DrawCube(bounds.center, bounds.size));
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(VirtualCameraSwitcher))]
    class MyEditor : Editor
    {
        VirtualCameraSwitcher Target => target as VirtualCameraSwitcher;

        void Draw(string prop) => EditorGUILayout.PropertyField(serializedObject.FindProperty(prop));

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField($"{vcams?.Length.ToString() ?? "(?)"} vcams, {instances.Count} switchers.");

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Follow (info)", follow, typeof(Transform), true);
            EditorGUILayout.ObjectField("Current Vcam (info)", currentVcam, typeof(CinemachineVirtualCamera), true);
            EditorGUILayout.ObjectField("Default Vcam (info)", defaultVcam, typeof(CinemachineVirtualCamera), true);
            GUI.enabled = true;

            Draw("vcam");
            Draw("onEnterPriority");
            Draw("safeMargin");
            Draw("gizmoColor");
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Reset Priority"))
            {
                FindVcams(true);
                foreach(var vcam in vcams)
                    vcam.Priority = vcam == defaultVcam ? 10 : 0;
            }

            if (GUILayout.Button("Update Priority"))
            {
                UpdatePriority(true);
            }
        }
    }
#endif
}
