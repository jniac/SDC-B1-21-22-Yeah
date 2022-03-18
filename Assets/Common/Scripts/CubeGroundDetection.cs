using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class CubeGroundDetection : MonoBehaviour
{
    public float corner = 0.41f;

    [Tooltip("Distance max pour être considéré comme \"grounded\"")]
    public float groundDistanceMax = 0.33f;
    [Tooltip("Distance max de détection (info)")]
    public float detectionDistanceMax = 1f;

    public LayerMask groundMask = 1; // = "Default"

    [System.NonSerialized, Tooltip("Distance réelle avec le sol")]
    public float groundDistance = float.PositiveInfinity;
    [System.NonSerialized]
    public bool onGround = false;
    [System.NonSerialized]
    public float onGroundTime = float.NegativeInfinity;
    [System.NonSerialized]
    public float timeSinceOnGround = float.PositiveInfinity;

    Vector3[] points = new Vector3[8];
    RaycastHit[] pointHits = new RaycastHit[8];
    RaycastHit groundHit = new RaycastHit();
    HashSet<RaycastHit> groundHits = new HashSet<RaycastHit>();
    HashSet<RaycastHit> triggerHits = new HashSet<RaycastHit>();

    void UpdatePoints()
    {
        points[0] = transform.TransformPoint(+corner, +corner, +corner);
        points[1] = transform.TransformPoint(-corner, +corner, +corner);
        points[2] = transform.TransformPoint(-corner, -corner, +corner);
        points[3] = transform.TransformPoint(+corner, -corner, +corner);
        points[4] = transform.TransformPoint(+corner, +corner, -corner);
        points[5] = transform.TransformPoint(-corner, +corner, -corner);
        points[6] = transform.TransformPoint(-corner, -corner, -corner);
        points[7] = transform.TransformPoint(+corner, -corner, -corner);
    }

    void UpdateGroundColliders()
    {
        groundHits.Clear();
        triggerHits.Clear();

        groundDistance = float.PositiveInfinity;
        groundHit = new RaycastHit { distance = float.PositiveInfinity };

        float yThreshold = transform.position.y;
        for (int index = 0; index < 8; index++)
        {
            var point = points[index];

            // Ignore "upper" points.
            if (point.y > yThreshold)
                continue;

            var hits = Physics.RaycastAll(point, Vector3.down, detectionDistanceMax, groundMask, QueryTriggerInteraction.Collide);
            float minDistance = float.PositiveInfinity;
            pointHits[index] = new RaycastHit { distance = float.PositiveInfinity };
            foreach (var hit in hits)
            {
                // "Normal / Hard / Plain" colliders: 
                if (hit.collider.isTrigger == false)
                {
                    if (hit.distance < minDistance)
                    {
                        minDistance = hit.distance;
                        pointHits[index] = hit;
                    }

                    if (hit.distance < groundDistance)
                    {
                        groundDistance = hit.distance;
                        groundHit = hit;
                    }

                    groundHits.Add(hit);
                }

                // "Trigger / Soft" colliders:
                else
                {
                    triggerHits.Add(hit);
                }
            }
        }

        onGround = groundDistance < groundDistanceMax;

        if (onGround)
            onGroundTime = Time.time;

        timeSinceOnGround = Time.time - onGroundTime;
    }

    void Update()
    {
        UpdatePoints();
        UpdateGroundColliders();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        System.Action<float, System.Action> WithAlpha = (alpha, action) => {
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, alpha);
            action();
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 1f);
        };

        // Corners
        float yThreshold = transform.position.y;
        for (int index = 0; index < 8; index++)
        {
            var point = points[index];

            Gizmos.DrawSphere(point, 0.05f);

            if (point.y > yThreshold)
                continue;

            var groundMaxPoint = point + Vector3.down * groundDistanceMax;
            var detectionMaxPoint = point + Vector3.down * detectionDistanceMax;
            Gizmos.DrawSphere(groundMaxPoint, 0.015f);
            Gizmos.DrawLine(point, groundMaxPoint);
            WithAlpha(0.33f, () => Gizmos.DrawLine(groundMaxPoint, detectionMaxPoint));

            if (pointHits[index].distance < groundDistanceMax)
                Gizmos.DrawSphere(pointHits[index].point, 0.05f);
        }

        // Ground Hit
        if (groundHit.distance <= groundDistanceMax)
        {
            Gizmos.DrawWireSphere(groundHit.point, 0.06f);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CubeGroundDetection))]
    class MyEditor : Editor
    {
        CubeGroundDetection Target => target as CubeGroundDetection;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.enabled = false;
            EditorGUILayout.Slider("Ground Distance", Target.groundDistance, 0f, Target.groundDistanceMax);
            EditorGUILayout.Toggle("On Ground", Target.onGround);
            EditorGUILayout.FloatField("On Ground Time", Target.onGroundTime);
            EditorGUILayout.FloatField("Time Since On Ground", Target.timeSinceOnGround);
        }
    }
#endif
}
