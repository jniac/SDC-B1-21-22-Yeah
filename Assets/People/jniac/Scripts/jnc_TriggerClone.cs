using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_TriggerClone : MonoBehaviour
{
    public LayerMask mask = ~0;
    public float cooldownDuration = 0.1f;

    float cloneTime = 0;

    bool Match(int layer) => (mask & (1 << layer)) != 0;

    void OnTriggerEnter(Collider other)
    {
        var source = other.attachedRigidbody.gameObject;

        if (Match(source.layer)) {

            var cooling = Time.time - cloneTime < cooldownDuration;
            
            if (cooling == false)
            {
                cloneTime = Time.time;
                Instantiate(source);
            }
        }
    }
}
