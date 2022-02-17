using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jniac_TriggerClone : MonoBehaviour
{
    public float cooldownDuration = 0.1f;

    float cloneTime = 0;

    void OnTriggerEnter(Collider other)
    {
        var source = other.attachedRigidbody.gameObject;
        var cooling = Time.time - cloneTime < cooldownDuration;
        if (cooling == false)
        {
            cloneTime = Time.time;
            Instantiate(source);
        }
    }
}
