using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AdjustParticleSystemScale : MonoBehaviour
{
    #if UNITY_EDITOR
    void Update()
    {
        if (Application.isPlaying == false)
        {
            var system = GetComponent<ParticleSystem>();
            var shape = system.shape;
            shape.scale = transform.parent.localScale;
        }
    }
    #endif
}
