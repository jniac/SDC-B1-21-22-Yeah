using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AdjustParticleSystemScale : MonoBehaviour
{
    public float rateByUnitSquare = 15f;

    #if UNITY_EDITOR
    void Update()
    {
        if (Application.isPlaying == false)
        {
            var system = GetComponent<ParticleSystem>();

            var shape = system.shape;
            shape.scale = transform.parent.localScale;

            var x = transform.parent.localScale.x;
            var z = transform.parent.localScale.x;
            var emission = system.emission;
            emission.rateOverTime = x * z * rateByUnitSquare;
        }
    }
    #endif
}
