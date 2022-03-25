using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class jnc_CameraSwitcher : MonoBehaviour
{
    static List<jnc_CameraSwitcher> instances = new List<jnc_CameraSwitcher>();

    static CinemachineVirtualCamera[] vcams;
    static Dictionary<CinemachineVirtualCamera, int> initialPriorities = new Dictionary<CinemachineVirtualCamera, int>();
    static bool startPriorityDone = false;
    static void StartPriority(bool force = false)
    {
        if (startPriorityDone && force == false)
            return;

        startPriorityDone = true;

        vcams = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var vcam in vcams)
            initialPriorities.Add(vcam, vcam.Priority);
    }

    static int updatePriorityFrame = -1;
    static void UpdatePriority()
    {
        if (Time.frameCount % 10 != 0)
            return;

        bool alreadyUpdated = updatePriorityFrame == Time.frameCount;

        if (alreadyUpdated)
            return;

        updatePriorityFrame = Time.frameCount;

        var follow = instances
            .Select(item => item.vcam?.Follow)
            .Where(item => item != null)
            .FirstOrDefault();

        if (follow == null)
            return;

        // Compute the maximum priority for each vcam.
        var vcamPriority = new Dictionary<CinemachineVirtualCamera, int>();
        foreach (var instance in instances)
        {
            // Clean dead references (could be destroyed).
            instance.overlapping.RemoveAll(t => t == null);

            bool overlaps = instance.overlapping.Contains(follow);
            int priority = overlaps ? instance.onEnterPriority : (initialPriorities.ContainsKey(instance.vcam) ? initialPriorities[instance.vcam] : 0);

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
            entry.Key.Priority = entry.Value;
    }

    public CinemachineVirtualCamera vcam;
    public int onEnterPriority = 20;

    List<Transform> overlapping = new List<Transform>();

    void OnEnable()
    {
        instances.Add(this);
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    void OnTriggerEnter(Collider other)
    {
        overlapping.Add(other.attachedRigidbody.transform);
    }

    void OnTriggerExit(Collider other)
    {
        overlapping.Remove(other.attachedRigidbody.transform);
    }

    void Start()
    {
        StartPriority();
    }

    void Update()
    {
        UpdatePriority();
    }

    void OnValidate()
    {
        var str = vcam != null ? vcam.name.Substring(vcam.name.Length - 5) : "...";
        gameObject.name = $"CameraSwitcher({str}:{onEnterPriority})";
    }

    public Color gizmoColor = Color.yellow;
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
    void OnDrawGizmosSelected()
    {
        var size = transform.localScale;
        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        GizmoPrimitives.WithAlpha(0.2f, () => Gizmos.DrawCube(Vector3.zero, Vector3.one));
    }

}
