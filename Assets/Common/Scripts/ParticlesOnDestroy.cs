using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesOnDestroy : MonoBehaviour
{
    public GameObject[] onDestroyParticles;

    public int particleCount = 30;

    void OnDestroy()
    {
        Boom.FromPoint(transform.position, onDestroyParticles, particleCount);
    }
}
