using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode, RequireComponent(typeof(EditingBlockSnapping))]
public class VirtualCameraSwitcher : MonoBehaviour
{
    class OverlapObservable
    {
        bool overlap = false;
        float enterTime = -1;
        float exitTime = -1;

        public bool Overlap
        {
            get => overlap;
            set => SetOverlap(value);
        }

        public bool GetOverlap() => overlap;

        public bool GetOverlapWithDelay(float delay) => overlap || (Time.time - exitTime < delay);
  
        public bool SetOverlap(bool value)
        {
            if (value != overlap)
            {
                overlap = value;

                if (overlap)
                    enterTime = Time.time;
                else
                    exitTime = Time.time;

                return true;
            }

            return false;
        }
    }

    static List<VirtualCameraSwitcher> instances = new List<VirtualCameraSwitcher>();
    static CinemachineVirtualCamera[] vcams;
    static CinemachineVirtualCamera currentVcam;
    static CinemachineVirtualCamera defaultVcam;
    static Transform follow;
    static void FindVcams(bool force = false)
    {
        if (vcams != null && force == false)
            return;

        vcams = FindObjectsOfType<CinemachineVirtualCamera>();

        // The default vcam is the first vcam that is not controlled 
        // by a VirtualCameraSwitcher instance.
        defaultVcam = vcams
            .Where(vcam => instances.All(switcher => switcher.vcam != vcam))
            .FirstOrDefault();
    }

    static int updatePriorityFrame = -1;
    static int updatePriorityCount = 0;
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
            // Ignore null vcam, important!
            if (instance.vcam == null)
                continue;

            instance.UpdateOverlap(follow.position);

            int priority = instance.Overlaps() ?
                instance.onEnterPriority :
                defaultVcam ? defaultVcam.Priority - 2 : 10;

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

        // NOTE: THIS IS AN HACK!
        // Since Cinemachine do not always take vcam priority into consideration 
        // (a bug), we have to update regulary the priority with a different number
        // (to trigger Cinemachine brain inner update).
        int forceCinemachineUpdateOffset = updatePriorityCount % 2;

        // Update each vcam with its previously computed priority.
        foreach (var entry in vcamPriority)
        {
            var vcam = entry.Key;
            int priority = entry.Value;
            vcam.Priority = priority + forceCinemachineUpdateOffset;
        }

        var newVcam = vcams.OrderBy(vcam => vcam.Priority).LastOrDefault();
        if (newVcam != currentVcam)
            Player.BroadcastAll("OnSwitchCamera", newVcam);
        currentVcam = newVcam;

        updatePriorityCount++;
    }

    public CinemachineVirtualCamera vcam;
    public int onEnterPriority = 20;
    public float exitDelay = 0f;
    public Vector3 safeMargin = Vector3.one * 0.5f;

    public Bounds Bounds => new Bounds(transform.position, transform.localScale + safeMargin);

    OverlapObservable overlap = new OverlapObservable();

    void UpdateName()
    {
        var str = vcam != null 
            ? Regex.Split(vcam.name, @"\W").Last()
            : "NO-CAM";
        gameObject.name = $"SwitchTo [{str} : {onEnterPriority}]";
    }

    void UpdateOverlap(Vector3 point)
    {
        overlap.SetOverlap(Bounds.Contains(point));
    }

    public bool Overlaps()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
            return overlap.GetOverlap();
#endif
        return overlap.GetOverlapWithDelay(exitDelay);
    }

    void OnEnable()
    {
        instances.Add(this);
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    void FixedUpdate()
    {
        UpdatePriority();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            UpdatePriority();
            UpdateName();
        }
#endif
    }

    void OnValidate()
    {
        UpdateName();
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
            Draw("exitDelay");
            Draw("safeMargin");
            Draw("gizmoColor");
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Manual Update Priority"))
            {
                UpdatePriority(true);
            }
        }
    }
#endif
}
