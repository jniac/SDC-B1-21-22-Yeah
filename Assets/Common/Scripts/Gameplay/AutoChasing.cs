using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chaser))]
public class AutoChasing : MonoBehaviour
{
    public float distanceMax = 7f;
    public float heightMax = 3f;

    Chaser chaser;

    void Start()
    {
        chaser = GetComponent<Chaser>();
    }

    Chaser.ChaseMode GetChaseMode()
    {
        if (chaser.target != null)
        {
            Vector3 delta = chaser.target.position - chaser.transform.position;
            float dy = Mathf.Abs(delta.y);
            float dxz = delta.x * delta.x + delta.z * delta.z;

            if (dy < heightMax && dxz < distanceMax * distanceMax)
            {
                return Chaser.ChaseMode.Chasing;
            }
        }

        return Chaser.ChaseMode.Idle;
    }

    void Update()
    {
        chaser.mode = GetChaseMode();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        GizmosUtils.DrawCylinder(
            distanceMax,
            heightMax * 2,
            ringSubdivisions: 4,
            center: transform.position,
            orientation: GizmosUtils.Orientation.XZ);
    }
}
