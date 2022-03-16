using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public LayerMask mask = 0;

    public bool destroyOnTrigger = true;
    public bool destroyOnCollision = true;

    bool Match(int layer)
    {
        return (mask & (1 << layer)) != 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (destroyOnTrigger)
        {
            Rigidbody body = other.attachedRigidbody;
            
            if (body != null)
            {
                if (Match(body.gameObject.layer))
                {
                    Destroy(body.gameObject);
                }
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (destroyOnCollision)
        {
            Rigidbody body = other.collider.attachedRigidbody;
            
            if (body != null)
            {
                if (Match(body.gameObject.layer))
                {
                    Destroy(body.gameObject);
                }
            }
        }
    }
}
