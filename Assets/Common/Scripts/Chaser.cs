using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CubeGroundDetection))]
public class Chaser : MonoBehaviour
{
    public enum ChaseMode
    {
        Idle,
        Chasing,
    }

    public ChaseMode mode = ChaseMode.Idle;
    public string targetTag = "Player";
    public Transform target;
    public float velocity = 3f;

    Rigidbody body;
    CubeGroundDetection groundDetection;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        groundDetection = Utils.RequireComponent<CubeGroundDetection>(gameObject);
    }

    void Chase()
    {
        if (groundDetection.onGround)
        {
            Vector3 v = target.position - transform.position;

            v = v.normalized * velocity;

            // Conservation de la vitesse verticale (gravité).
            v.y = body.velocity.y;

            body.velocity = Vector3.Lerp(body.velocity, v, 0.5f);
        }
    }

    void Update()
    {
        if (target == null)
        {
            // Si la cible est nulle, tentative de récupérer une cible dynamiquement.
            target = GameObject.FindGameObjectWithTag(targetTag)?.transform;
        }

        if (target != null)
        {
            if (mode == ChaseMode.Chasing)
            {
                Chase();
            }
        }

    }
}
