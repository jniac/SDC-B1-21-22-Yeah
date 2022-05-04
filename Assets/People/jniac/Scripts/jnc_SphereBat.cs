using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class jnc_SphereBat : MonoBehaviour
{
    static int updateTargetDelay = 20;

    public float radius = 12f;
    public float velocity = 3.5f;

    Transform target;
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        bool updateTarget = (Time.frameCount % updateTargetDelay) == 0;

        if (updateTarget)
        {
            var p = transform.position;
            var sqRadius = radius * radius;

            target = GameObject
                .FindGameObjectsWithTag("Player")
                .Where(go => (go.transform.position - p).sqrMagnitude < sqRadius)
                .FirstOrDefault()
                ?.transform;
        }

        if (target && target.gameObject.tag == "Player")
        {
            body.velocity = (target.position - transform.position).normalized * velocity;
        }
        else
        {
            body.velocity *= 0.95f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
