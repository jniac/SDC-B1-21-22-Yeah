using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class jnc_CameraSwitcher : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera vcam1;
    public Cinemachine.CinemachineVirtualCamera vcam2;

    List<Transform> overlapping = new List<Transform>();

    void SetPriority(bool toVcam1)
    {
        if (toVcam1)
        {
            vcam1.Priority = 1;
            vcam2.Priority = 0;
        }
        else
        {
            vcam1.Priority = 0;
            vcam2.Priority = 1;
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

    void Update()
    {
        bool update = Time.frameCount % 10 == 0
            && vcam1.Follow != null;

        if (update)
        {
            // Clean dead references (could be destroyed).
            overlapping.RemoveAll(t => t == null);

            bool containsCameraTarget = overlapping.Any(t => vcam1.Follow == t);
            SetPriority(containsCameraTarget == false);
        }
    }
}
