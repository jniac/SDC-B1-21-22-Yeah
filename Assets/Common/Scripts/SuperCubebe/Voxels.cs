using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SuperCubebe
{
    public enum VoxelType
    {
        VOID = 0,
        PLAIN = 1,
    }
    
    public struct Voxel
    {
        public VoxelType type;
    }

    public class VoxelWorld
    {
        public BoundsInt bounds;
        public Voxel[,,] voxels;
        public readonly int x, y, z;
        public readonly int sizeX, sizeY, sizeZ;

        public VoxelWorld(IEnumerable<Component> components)
        {
            var allBounds = components.Select(component => Utils.ToBounds(component)).ToArray();

            bounds = Utils.BoundsUnion(allBounds);
            x = bounds.xMin;
            y = bounds.yMin;
            z = bounds.zMin;
            sizeX = bounds.size.x;
            sizeY = bounds.size.y;
            sizeZ = bounds.size.z;
            voxels = new Voxel[sizeX, sizeY, sizeZ];

            foreach (var currentBounds in allBounds)
            {
                foreach (var p in currentBounds.allPositionsWithin)
                {
                    voxels[p.x - x, p.y - y, p.z - z].type = VoxelType.PLAIN;
                }
            }
        }

        public IEnumerable<(Voxel voxel, Vector3Int position)> AllVoxels()
        {
            foreach (var p in bounds.allPositionsWithin)
            {
                var voxel = voxels[p.x - x, p.y - y, p.z - z];
                yield return (voxel, p);
            }
        }
    }

    public class Utils
    {
        public static BoundsInt ToBounds(GameObject gameObject) => ToBounds(gameObject.transform);
        public static BoundsInt ToBounds(Component component) => ToBounds(component.transform);
        public static BoundsInt ToBounds(Transform transform)
        {
            var p = transform.position;
            var s = transform.localScale;
            int sx = Mathf.RoundToInt(s.x);
            int sy = Mathf.RoundToInt(s.y);
            int sz = Mathf.RoundToInt(s.z);
            int px = Mathf.RoundToInt(p.x - (float)sx / 2f);
            int py = Mathf.RoundToInt(p.y - (float)sy / 2f);
            int pz = Mathf.RoundToInt(p.z - (float)sz / 2f);
            return new BoundsInt(px, py, pz, sx, sy, sz);
        }

        public static BoundsInt BoundsUnion(IEnumerable<GameObject> gameObjects) => BoundsUnion(gameObjects.Select(gameObject => ToBounds(gameObject)));
        public static BoundsInt BoundsUnion(IEnumerable<Component> components) => BoundsUnion(components.Select(component => ToBounds(component)));
        public static BoundsInt BoundsUnion(IEnumerable<Transform> transforms) => BoundsUnion(transforms.Select(transform => ToBounds(transform)));
        public static BoundsInt BoundsUnion(IEnumerable<BoundsInt> boundss)
        {
            var total = new BoundsInt(0, 0, 0, -1, -1, -1);

            BoundsInt bounds;

            var iter = boundss.GetEnumerator();

            if (iter.MoveNext())
            {
                total = iter.Current; // First time, set "Total".
            }

            while (iter.MoveNext())
            {
                bounds = iter.Current;
                total.xMin = Mathf.Min(total.xMin, bounds.xMin);
                total.yMin = Mathf.Min(total.yMin, bounds.yMin);
                total.zMin = Mathf.Min(total.zMin, bounds.zMin);
                total.xMax = Mathf.Max(total.xMax, bounds.xMax);
                total.yMax = Mathf.Max(total.yMax, bounds.yMax);
                total.zMax = Mathf.Max(total.zMax, bounds.zMax);
            }

            return total;
        }

        public static IEnumerable<Vector3Int> PositionsInBounds(BoundsInt bounds)
        {
            int x = bounds.xMin;
            int y = bounds.yMin;
            int z = bounds.zMin;
            int xMax = bounds.xMax;
            int yMax = bounds.yMax;
            int zMax = bounds.zMax;

            while (z < zMax)
            {
                while (y < yMax)
                {
                    while (x < xMax)
                    {
                        yield return new Vector3Int(x, y, z);
                        x++;
                    }
                    y++;
                }
                z++;
            }
        }
    }
}