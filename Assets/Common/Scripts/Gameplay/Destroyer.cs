using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public LayerMask mask = ~0;

    public enum DestroyCondition
    {
        Trigger,
        Collision,
        Both,
    }

    public DestroyCondition destroyCondition = DestroyCondition.Trigger;

    public bool autoDestroy = false;

    bool Match(int layer) => (mask & (1 << layer)) != 0;

    void DestroyIt(GameObject other)
    {
        // Si invicible, alors invicible (return).
        if (other.tag == "Player" && PlayModeManager.Test(PlayMode.NeverDie))
            return;

        if (other.TryGetComponent<Health>(out var health))
        {
            health.ApplyDamage(1000);
        }
        else
        {
            Destroy(other);
        }

        if (autoDestroy)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (destroyCondition == DestroyCondition.Trigger || destroyCondition == DestroyCondition.Both)
        {
            Rigidbody body = other.attachedRigidbody;

            if (body != null)
            {
                if (Match(body.gameObject.layer))
                {
                    DestroyIt(body.gameObject);
                }
            }
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (destroyCondition == DestroyCondition.Collision || destroyCondition == DestroyCondition.Both)
        {
            Rigidbody body = other.collider.attachedRigidbody;

            if (body != null)
            {
                if (Match(body.gameObject.layer))
                {
                    DestroyIt(body.gameObject);
                }
            }
        }
    }
}
