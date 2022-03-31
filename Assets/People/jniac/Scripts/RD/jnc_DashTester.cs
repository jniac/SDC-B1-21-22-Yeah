using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class jnc_DashTester : MonoBehaviour
{
    [Range(1, 5)]
    public float dashLength = 3.6f;
    public float radius = 0.5f;

    Vector3[] spreadVectors = new Vector3[0];
    jnc_CubeDash.DashInfo info;

    SuperCubebe.VoxelWorld world;

    void OnEnable()
    {
        world = SuperCubebe.VoxelWorld.New(FindObjectsOfType<EditingBlockSnapping>());        
    }

    void Update()
    {
        spreadVectors = jnc_CubeDash.GetSpreadVectors(transform.right);
        info = jnc_CubeDash.DestinationCast(transform.position, transform.right, dashLength, radius, 1);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.right * dashLength);
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(transform.position + transform.right * dashLength, 0.1f);

        foreach (var v in spreadVectors)
            Gizmos.DrawRay(transform.position, v * dashLength);

        Gizmos.DrawWireCube(transform.position, Vector3.one);
        GizmosUtils.WithAlpha(0.1f, () =>
            Gizmos.DrawCube(transform.position, Vector3.one));

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, info.direction * dashLength);
        Gizmos.DrawSphere(info.destination, 0.1f);
        Gizmos.DrawWireSphere(info.destination, radius);
    }
}
