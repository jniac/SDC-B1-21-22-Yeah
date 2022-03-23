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
        initialPriority = vcam.Priority;
    }

    void Update()
    {
        bool update = Time.frameCount % 10 == 0
            && vcam.Follow != null;

        if (update)
        {
            // Clean dead references (could be destroyed).
            overlapping.RemoveAll(t => t == null);

            bool containsCameraTarget = overlapping.Any(t => vcam.Follow == t);
            SetPriority(containsCameraTarget);
        }
    }
}
