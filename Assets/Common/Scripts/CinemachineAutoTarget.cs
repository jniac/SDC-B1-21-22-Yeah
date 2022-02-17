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

        var vc = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (vc != null)
            if (vc.Follow == null)
                vc.Follow = transform;
    }

    void OnDestroy()
    {
        instances.Remove(this);

        var vc = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (vc != null)
            if (vc.Follow == transform)
                if (instances.Count > 0)
                    vc.Follow = instances[0].transform;
    }
}
