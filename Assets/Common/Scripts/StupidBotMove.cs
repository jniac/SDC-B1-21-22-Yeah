using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidBotMove : MonoBehaviour
{
    public float velocity = 3f;
    public Transform target;

    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (target == null)
        {
            // Si la cible est nulle, tentative de récupérer une cible dynamiquement.
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (target != null) 
        {
            Vector3 direction = target.position - transform.position;

            direction = direction.normalized * velocity;

            // Conservation de la vitesse verticale (gravité).
            direction.y = body.velocity.y;

            body.velocity = direction;
        }
    }
}
