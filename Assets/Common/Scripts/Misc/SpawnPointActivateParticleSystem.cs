using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointActivateParticleSystem : MonoBehaviour
{
    ParticleSystem ps;

    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Pause();
    }

    void SpawnPointFocusEnter()
    {
        ps.Play();
    }

    void SpawnPointFocusExit()
    {
        ps.Clear();
        ps.Pause();
    }
}
