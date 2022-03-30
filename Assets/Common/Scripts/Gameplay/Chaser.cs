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
    public float updateTargetCooldown = 0.3f;

    Rigidbody body;
    CubeGroundDetection groundDetection;
    float updateTargetTime = -1;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        groundDetection = GetComponent<CubeGroundDetection>();
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

    void UpdateTarget()
    {
        updateTargetTime = Time.time;

        var candidates = GameObject.FindGameObjectsWithTag(targetTag);

        if (candidates.Length == 0)
        {
            target = null;
            return;
        }

        var p = transform.position;
        var candidate = candidates[0];
        var sqDistance = (candidate.transform.position - p).sqrMagnitude;
        target = candidate.transform;
        for (int index = 1; index < candidates.Length; index++)
        {
            var current = candidates[index];
            var currentSqDistance = (current.transform.position - p).sqrMagnitude;
            if (currentSqDistance < sqDistance)
            {
                target = current.transform;
                sqDistance = currentSqDistance;
            }
        }
    }

    void Update()
    {
        if (Time.time - updateTargetTime > updateTargetCooldown)
        {
            // Mettre à jour la cible à intervalles régulières. 
            UpdateTarget();
        }

        if (target != null && target.tag != targetTag)
        {
            // Si le tag a changé, la cible n'est plus valide.
            UpdateTarget();
        }

        if (target != null && target.tag == targetTag)
        {
            if (mode == ChaseMode.Chasing)
            {
                Chase();
            }
        }

    }
}
