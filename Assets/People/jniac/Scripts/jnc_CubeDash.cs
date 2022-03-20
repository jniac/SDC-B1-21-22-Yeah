using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CubeMove))]
public class jnc_CubeDash : MonoBehaviour
{
    public float minVelocityToDash = 0.1f;
    public float cooldownDuration = 0.3f;
    public float dashLength = 2f;
    public float colliderRadius = 0.5f;

    public bool triggerDash = false;

    public LayerMask dashObstacleMask = 1; // 1 == Default

    public GameObject[] onDestroyParticles;

    DashRequestStatus requestStatus;
    DashStatus status;
    Vector3 direction;
    float dashTime = -1;

    public enum DashRequestStatus
    {
        NoDirection,
        TooSoon,
        Ok,
    }

    public enum DashStatus
    {
        HitWall,
        NoObstaclesBut,
        NoObstacles,
    }

    (DashRequestStatus status, Vector3 direction) CanDash()
    {
        if (Time.time - dashTime < cooldownDuration)
            return (DashRequestStatus.TooSoon, Vector3.zero);

        var direction = GetComponent<CubeMove>().InputVelocity;

        if (direction.magnitude < minVelocityToDash)
            return (DashRequestStatus.NoDirection, Vector3.zero);

        return (DashRequestStatus.Ok, direction.normalized);
    }

    void Dash()
    {
        dashTime = Time.time;

        var body = GetComponent<Rigidbody>();

        Vector3 destination = body.position + direction * dashLength;

        float tolerance = 0.9f;
        if (Physics.OverlapSphere(destination, colliderRadius * tolerance, dashObstacleMask).Length > 0)
        {
            if (Physics.Raycast(body.position, direction, out var hit, dashLength + colliderRadius, dashObstacleMask))
            {
                // Normal case: a little back from the hit point.
                destination = hit.point - direction * colliderRadius;
                status = DashStatus.HitWall;
            }
            else
            {
                // Rare edge case: the overlap collider is not cast by the ray 
                // (may occurs during jumps)
                status = DashStatus.NoObstaclesBut;
            }
        }
        else
        {
            // No walls at all. Go straight.
            status = DashStatus.NoObstacles;
        }

        body.position = destination;
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Dash") || triggerDash)
        {
            var result = CanDash();
            requestStatus = result.status;
            direction = result.direction;

            if (requestStatus == DashRequestStatus.Ok)
                Dash();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + direction * dashLength, 0.25f);
    }
}
