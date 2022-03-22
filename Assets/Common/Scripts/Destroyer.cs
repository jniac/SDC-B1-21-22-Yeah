using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public LayerMask mask = ~0;

    public enum DestroyCondition {
        Trigger,
        Collision,
        Both,
    }

    public DestroyCondition destroyCondition = DestroyCondition.Trigger;

    bool Match(int layer) => (mask & (1 << layer)) != 0;

    void OnTriggerEnter(Collider other)
    {
        if (destroyCondition == DestroyCondition.Trigger || destroyCondition == DestroyCondition.Both)
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
        if (destroyCondition == DestroyCondition.Collision || destroyCondition == DestroyCondition.Both)
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
