using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloner : MonoBehaviour
{
    public LayerMask mask = ~0;

    public bool cloneOnTrigger = true;
    public bool cloneOnCollision = true;

    [Tooltip("Nombre de clone avant auto-destruction. Si -1: Pas de limite.")]
    public int count = -1;

    public float cooldownDuration = 0.1f;

    float cloneTime = 0;

    bool Match(int layer) => (mask & (1 << layer)) != 0;

    void Clone(GameObject source)
    {
        var cooling = Time.time - cloneTime < cooldownDuration;

        if (cooling == false)
        {
            cloneTime = Time.time;
            Instantiate(source);

            count += -1;
            if (count == 0)
                Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (cloneOnTrigger)
        {
            var source = other.attachedRigidbody.gameObject;
            if (Match(source.layer))
                Clone(source);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (cloneOnCollision)
        {
            var source = other.collider.attachedRigidbody.gameObject;
            if (Match(source.layer))
                Clone(source);
        }
    }
}
