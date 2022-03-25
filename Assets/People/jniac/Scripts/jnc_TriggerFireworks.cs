using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_TriggerFireworks : MonoBehaviour
{
    public GameObject[] sources = new GameObject[0];

    public int particuleCount = 10;
    public float velocity = 10;
    public float lifeDuration = 1.5f;


    void OnTriggerEnter(Collider other)
    {
        Boom.FromPoint(other.gameObject.transform.position, sources, particuleCount, lifeDuration, velocity);
    }
}
