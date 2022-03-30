using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class jnc_BoundsIntersection : MonoBehaviour
{
    SuperCubebe.VoxelWorld world;

    void OnEnable()
    {
        world = new SuperCubebe.VoxelWorld(FindObjectsOfType<EditingBlockSnapping>());        
    }

    void OnDrawGizmos()
    {
        var p = transform.position;
        var s = Vector3Int.one * 4;
        var bounds = new BoundsInt(
            Mathf.RoundToInt(p.x - s.x / 2f),
            Mathf.RoundToInt(p.y - s.y / 2f),
            Mathf.RoundToInt(p.z - s.z / 2f),
            s.x, s.y, s.z);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(world.bounds.center, world.bounds.size);
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (SuperCubebe.Utils.BoundsIntersection(world.bounds, bounds, out var I))
        {
            GizmosUtils.WithAlpha(0.25f, () => {
                Gizmos.DrawCube(I.center, I.size);
            });
        }
    }
}
