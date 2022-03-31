using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class jnc_VoxelTester : MonoBehaviour
{
    [Range(0, 100)]
    public int skip = 0;

    SuperCubebe.VoxelWorld world;

    void Update()
    {
        world = SuperCubebe.VoxelWorld.New(FindObjectsOfType<EditingBlockSnapping>());
    }

    void DrawPlain()
    {
        GizmosUtils.WithAlpha(0.3f, () => {
            foreach (var voxel in world.VoxelViews())
            {
                if (voxel.voxel.type == SuperCubebe.VoxelType.PLAIN)
                    Gizmos.DrawCube(voxel.Center, Vector3.one);
            }
        });
    }

    void Draw1()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(world.bounds.center, world.bounds.size);

        foreach (var face in world.FaceViews())
        {
            Gizmos.DrawSphere(face.Center, 0.025f);
            Gizmos.DrawRay(face.Center, face.Normal * 0.5f);
            
            Gizmos.DrawWireCube(face.Center, face.Size);
        }

        {
            var face = world.FaceViews().Skip(skip).First();
            GizmosUtils.WithAlpha(0.5f, () =>
                Gizmos.DrawCube(face.Center, face.Size));

            var (A, B, C, D) = face.Vertices;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(A, 0.075f);
            Gizmos.color = Color.Lerp(Color.red, Color.blue, 0.2f);
            Gizmos.DrawSphere(B, 0.075f);
            Gizmos.color = Color.Lerp(Color.red, Color.blue, 0.4f);
            Gizmos.DrawSphere(C, 0.075f);
            Gizmos.color = Color.Lerp(Color.red, Color.blue, 0.6f);
            Gizmos.DrawSphere(D, 0.075f);
        }
    }

    void OnDrawGizmos()
    {
        Draw1();

        // var p = transform.position;
        // var s = Vector3Int.one * 4;
        // var bounds = new BoundsInt(
        //     Mathf.RoundToInt(p.x - s.x / 2f),
        //     Mathf.RoundToInt(p.y - s.y / 2f),
        //     Mathf.RoundToInt(p.z - s.z / 2f),
        //     s.x, s.y, s.z);
        
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireCube(world.bounds.center, world.bounds.size);
        // Gizmos.DrawWireCube(bounds.center, bounds.size);

        // if (SuperCubebe.Utils.BoundsIntersection(world.bounds, bounds, out var I))
        // {
        //     GizmosUtils.WithAlpha(0.25f, () => {
        //         Gizmos.DrawCube(I.center, I.size);
        //     });
        // }



        // Gizmos.DrawWireCube(world.bounds.center, world.bounds.size);

        // foreach (var voxel in world.AllVoxels())
        // {
        //     GizmosUtils.WithAlpha(0.3f, () =>
        //         Gizmos.DrawCube(voxel.Center, Vector3.one));
        // }

        // foreach (var face in world.AllFaces())
        // {
        //     Gizmos.DrawSphere(face.Position, 0.1f);
        // }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerSpawnPoint))]
    class MyEditor : Editor
    {
        PlayerSpawnPoint Target => target as PlayerSpawnPoint;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


        }
    }
#endif
}
