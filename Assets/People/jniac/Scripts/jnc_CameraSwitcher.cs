using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class jnc_CameraSwitcher : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera vcam;
    public int onEnterPriority = 20;

    int initialPriority = 0;
    List<Transform> overlapping = new List<Transform>();

    void SetPriority(bool enter)
    {
        if (enter)
        {
            vcam.Priority = onEnterPriority;
        }
        else
        {
            vcam.Priority = initialPriority;
        }
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
        initialPriority = vcam?.Priority ?? 0;
    }

    void Update()
    {
        bool update = (
            vcam != null
            && vcam.Follow != null
            && Time.frameCount % 10 == 0);

        if (update)
        {
            // Clean dead references (could be destroyed).
            overlapping.RemoveAll(t => t == null);

            bool containsCameraTarget = overlapping.Any(t => vcam.Follow == t);
            SetPriority(containsCameraTarget);
        }
    }

    void OnValidate()
    {
        var str = vcam != null ? vcam.name.Substring(vcam.name.Length - 5) : "...";
        gameObject.name = $"CameraSwitcher({str})";
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
