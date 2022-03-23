using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CinemachineAutoTarget : MonoBehaviour
{
    static List<CinemachineAutoTarget> instances = new List<CinemachineAutoTarget>();

    void Start()
    {
        instances.Add(this);

        foreach (var vcam in FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>())
        {
            if (vcam.Follow == null)
                vcam.Follow = transform;
        }
    }

    void OnDestroy()
    {
        instances.Remove(this);

        foreach (var vcam in FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>())
        {
            if (vcam.Follow == transform)
            {
                if (instances.Count > 0)
                {
                    vcam.Follow = instances
                        .OrderBy(target => (target.transform.position - transform.position).sqrMagnitude)
                        .First()
                        .transform;
                }
                else
                {
                    vcam.Follow = null;
                }
            }
        }
    }
}
