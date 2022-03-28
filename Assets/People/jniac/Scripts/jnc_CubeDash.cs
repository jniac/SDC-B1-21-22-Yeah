using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CubeMove), typeof(CubeGroundDetection))]
public class jnc_CubeDash : MonoBehaviour
{
    public float minVelocityToDash = 0.1f;
    public float cooldownDuration = 0.3f;
    public float dashLength = 3.6f;
    public float colliderRadius = 0.5f;
    public int airDashMax = 1;

    public bool dashKill = true;
    public LayerMask dashKillMask = 1 << 10;
    public float dashKillRadius = 0.8f;

    public bool triggerDash = false;

    public LayerMask dashObstacleMask = 1; // 1 == Default

    public GameObject[] onDestroyParticles;

    Rigidbody body;
    DashRequestStatus requestStatus;
    DashStatus status;
    Vector3 direction;
    float dashTime = -1;
    int airDashCount = 0;

    public enum DashRequestStatus
    {
        NoControls,
        AirDashNotAvailable,
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
        var move = GetComponent<CubeMove>();

        if (move.ControlsCoeff < 0.5f)
            return (DashRequestStatus.NoControls, Vector3.zero);

        if (GetComponent<CubeGroundDetection>().onGround == false && airDashCount == airDashMax)
            return (DashRequestStatus.AirDashNotAvailable, Vector3.zero);

        if (Time.time - dashTime < cooldownDuration)
            return (DashRequestStatus.TooSoon, Vector3.zero);

        var direction = move.InputVector3;
        if (direction.magnitude < minVelocityToDash)
            return (DashRequestStatus.NoDirection, Vector3.zero);

        return (DashRequestStatus.Ok, direction.normalized);
    }

    void DashKillEnemy()
    {
        var hits = Physics.SphereCastAll(body.position, dashKillRadius, direction, dashLength, dashKillMask);
        foreach (var hit in hits)
        {
            var go = hit.collider.attachedRigidbody.gameObject;
            Destroy(go);
        }

        BroadcastMessage("InvicibleUntil", 0.2f, SendMessageOptions.DontRequireReceiver);
    }

    (Vector3 vR, Vector3 vL, Vector3 vT, Vector3 vB) GetSpreadVectors(Vector3 v, float dispersionAngle = 12f)
    {
        var right = Vector3.Cross(Vector3.up, v);
        var vR = Quaternion.AngleAxis(+dispersionAngle, Vector3.up) * v;
        var vL = Quaternion.AngleAxis(-dispersionAngle, Vector3.up) * v;
        var vT = Quaternion.AngleAxis(+dispersionAngle, right) * v;
        var vB = Quaternion.AngleAxis(-dispersionAngle, right) * v;
        return (vR, vL, vT, vB);
    }

    Vector3[] GetSpreadVectorsArray(Vector3 v)
    {
        var (vR, vL, vT, vB) = GetSpreadVectors(v);
        return new Vector3[] { vR, vL, vT, vB };
    }

    (Vector3 destination, Vector3 direction, bool collides) DestinationCast()
    {
        var position = body.position;
        Vector3 destination = position + direction * dashLength;

        Collider[] test; 
        float tolerance = 0.9f;

        test = Physics.OverlapSphere(destination, colliderRadius * tolerance, dashObstacleMask);

        if (test.Length == 0)
            return (destination, direction, false);

        // Try with "spread" vectors.
        foreach(var vector in GetSpreadVectorsArray(direction))
        {
            destination = position + vector * dashLength;
            test = Physics.OverlapSphere(destination, colliderRadius * tolerance, dashObstacleMask);

            if (test.Length == 0)
                return (destination, vector, false);
        }

        return (destination, direction, true);
    }

    void Dash()
    {
        dashTime = Time.time;

        var (destination, newDirection, collides) = DestinationCast();
        direction = newDirection;

        if (collides)
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

        if (GetComponent<CubeGroundDetection>().onGround)
            airDashCount = 0;
        else
            airDashCount += 1;

        if (dashKill)
            DashKillEnemy();

        body.position = destination;
    }

    void OnGroundEnter()
    {
        airDashCount = 0;
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Dash") || triggerDash)
        {
            var result = CanDash();
            requestStatus = result.status;
            direction = result.direction;

            if (requestStatus == DashRequestStatus.Ok)
                Dash();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var d = GetComponent<CubeMove>().Direction;
        Gizmos.DrawSphere(transform.position + d * dashLength, 0.25f);
        foreach(var vector in GetSpreadVectorsArray(d))
            Gizmos.DrawSphere(transform.position + vector * dashLength, 0.25f);
    }
}
