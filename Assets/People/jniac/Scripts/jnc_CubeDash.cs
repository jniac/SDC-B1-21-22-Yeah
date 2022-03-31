using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CubeMove), typeof(CubeGroundDetection))]
public class jnc_CubeDash : MonoBehaviour
{
    public enum DashRequestStatus
    {
        NoControls,
        AirDashNotAvailable,
        NoDirection,
        NoSpace,
        TooSoon,
        Ok,
        OkViaAxisCorrection,
    }

    public enum DashStatus
    {
        HitWall,
        NoObstaclesBut,
        NoObstacles,
    }

    public struct DashInfo
    {
        public Vector3 origin;
        public Vector3 direction;
        public Vector3 destination;
        public DashStatus status;

        public Vector3 Delta => destination - origin;

        public DashInfo(Vector3 origin, Vector3 destination, Vector3 direction, DashStatus status)
        {
            this.origin = origin;
            this.destination = destination;
            this.direction = direction;
            this.status = status;
        }
    }

    public static Vector3[] GetSpreadVectors(Vector3 v, float horizontalDispersionAngle = 14f, float verticalDispersionAngle = 14f)
    {
        var right = Vector3.Cross(Vector3.up, v);

        return new Vector3[] {
            // Horizontal
            Quaternion.AngleAxis(+horizontalDispersionAngle * 0.15f, Vector3.up) * v,
            Quaternion.AngleAxis(-horizontalDispersionAngle * 0.15f, Vector3.up) * v,
            Quaternion.AngleAxis(-horizontalDispersionAngle * 0.50f, Vector3.up) * v,
            Quaternion.AngleAxis(+horizontalDispersionAngle * 0.50f, Vector3.up) * v,
            Quaternion.AngleAxis(-horizontalDispersionAngle * 1.00f, Vector3.up) * v,
            Quaternion.AngleAxis(+horizontalDispersionAngle * 1.00f, Vector3.up) * v,

            // Vertical
            Quaternion.AngleAxis(+verticalDispersionAngle * 0.50f, right) * v,
            Quaternion.AngleAxis(-verticalDispersionAngle * 0.50f, right) * v,
            Quaternion.AngleAxis(+verticalDispersionAngle * 1.00f, right) * v,
            Quaternion.AngleAxis(-verticalDispersionAngle * 1.00f, right) * v,
        };
    }

    public static DashInfo DestinationCast(
        Vector3 origin,
        Vector3 direction,
        float length,
        float radius,
        LayerMask mask
    )
    {
        Vector3 destination = origin + direction * length;

        Collider[] test;
        float tolerance = 0.9f;

        test = Physics.OverlapSphere(destination, radius * tolerance, mask);

        if (test.Length == 0)
            return Raycast(false, origin, destination, direction, length, radius, mask);

        // Try with "spread" vectors.
        foreach (var vector in GetSpreadVectors(direction))
        {
            destination = origin + vector * length;
            test = Physics.OverlapSphere(destination, radius * tolerance, mask);

            if (test.Length == 0)
                return Raycast(false, origin, destination, vector, length, radius, mask);
        }

        return Raycast(true, origin, destination, direction, length, radius, mask);
    }

    public static DashInfo Raycast(
        bool collides,
        Vector3 origin,
        Vector3 destination,
        Vector3 direction,
        float length,
        float radius,
        LayerMask mask
    )
    {
        if (collides)
        {
            if (Physics.Raycast(origin, direction, out var hit, length + radius, mask))
            {
                // Destination correction: a little back from the hit point.
                destination = hit.point - direction * radius;
                return new DashInfo(origin, destination, direction, DashStatus.HitWall);
            }
            else
            {
                // Rare edge case: the overlap collider is not cast by the ray 
                // (may occurs during jumps)
                return new DashInfo(origin, destination, direction, DashStatus.NoObstaclesBut);
            }
        }
        else
        {
            // No walls at all. Go straight.
            return new DashInfo(origin, destination, direction, DashStatus.NoObstacles);
        }
    }



    // Instance.

    public float minVelocityToDash = 0.1f;
    public float minDistanceToDash = 0.75f;
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

    public DashRequestStatus RequestStatus { get; private set; }
    public DashStatus Status { get; private set; }

    Rigidbody body;
    CubeMove move;

    DashInfo info;
    float dashTime = -1;
    int airDashCount = 0;

    void DashKillEnemy()
    {
        var hits = Physics.SphereCastAll(info.origin, dashKillRadius, info.direction, dashLength, dashKillMask);
        foreach (var hit in hits)
        {
            var go = hit.collider.attachedRigidbody?.gameObject ?? hit.collider.gameObject;
            Destroy(go);
        }

        BroadcastMessage("InvicibleUntil", 0.2f, SendMessageOptions.DontRequireReceiver);
    }

    (DashRequestStatus status, DashInfo info) CanDash()
    {
        if (move.ControlsCoeff < 0.5f)
            return (DashRequestStatus.NoControls, default);

        if (GetComponent<CubeGroundDetection>().onGround == false && airDashCount == airDashMax)
            return (DashRequestStatus.AirDashNotAvailable, default);

        if (Time.time - dashTime < cooldownDuration)
            return (DashRequestStatus.TooSoon, default);

        var direction = move.InputVector3;
        if (direction.magnitude < minVelocityToDash)
            return (DashRequestStatus.NoDirection, default);

        var info = DestinationCast(body.position, direction, dashLength, colliderRadius, dashObstacleMask);

        if (info.Delta.sqrMagnitude < minDistanceToDash * minDistanceToDash)
        {
            var axisDirection = new Vector3[] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back }
                .OrderBy(v => Vector3.Dot(v, direction))
                .Last();

            // Second chance with axis direction.
            info = DestinationCast(body.position, axisDirection, dashLength, colliderRadius, dashObstacleMask);

            if (info.Delta.sqrMagnitude < minDistanceToDash * minDistanceToDash)
                return (DashRequestStatus.NoSpace, default);
            else
                return (DashRequestStatus.OkViaAxisCorrection, info);
        }

        return (DashRequestStatus.Ok, info);
    }

    void Dash()
    {
        dashTime = Time.time;

        if (GetComponent<CubeGroundDetection>().onGround)
            airDashCount = 0;
        else
            airDashCount += 1;

        if (dashKill)
            DashKillEnemy();

        body.position = info.destination;
    }

    void OnGroundEnter()
    {
        airDashCount = 0;
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        move = GetComponent<CubeMove>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Dash") || triggerDash)
        {
            (RequestStatus, info) = CanDash();

            Debug.Log(RequestStatus);

            if (RequestStatus == DashRequestStatus.Ok || RequestStatus == DashRequestStatus.OkViaAxisCorrection)
                Dash();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(info.origin, info.destination);
        Gizmos.DrawSphere(info.origin, 0.1f);
        Gizmos.DrawSphere(info.destination, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (move != null)
            foreach (var v in GetSpreadVectors(move.InputVector3))
                Gizmos.DrawRay(transform.position, v * dashLength);
    }
}
