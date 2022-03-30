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

    public struct VoxelView
    {
        public Voxel voxel;
        public Vector3Int position;

        public Vector3 Center => (Vector3)position + Vector3.one / 2f;
    }

    public struct FaceView
    {
        public Voxel voxelPlain, voxelVoid;
        public Vector3Int positionPlain, positionVoid;
        public Vector3 Center => ((Vector3)positionVoid + (Vector3)positionPlain) / 2f + Vector3.one / 2f;
        public Vector3 Normal => positionVoid - positionPlain;
        public Vector3 Size {
            get {
                var n = Normal;
                return new Vector3(
                    1f - Mathf.Abs(n.x), 
                    1f - Mathf.Abs(n.y), 
                    1f - Mathf.Abs(n.z));
            }
        }

        public void Set(bool voxel1IsPlain, Voxel voxel1, Vector3Int position1, Voxel voxel2, Vector3Int position2)
        {
            if (voxel1IsPlain)
            {
                voxelPlain = voxel1;
                positionPlain = position1;
                voxelVoid = voxel2;
                positionVoid = position2;
            }
            else
            {
                voxelPlain = voxel2;
                positionPlain = position2;
                voxelVoid = voxel1;
                positionVoid = position1;
            }
        }
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

        public IEnumerable<VoxelView> AllVoxels()
        {
            VoxelView voxel = default;

            foreach (var p in bounds.allPositionsWithin)
            {
                voxel.position = p;
                voxel.voxel = voxels[p.x - x, p.y - y, p.z - z];
                yield return voxel;
            }
        }

        public IEnumerable<FaceView> AllFaces()
        {
            var smallerBounds = bounds;
            smallerBounds.xMax += -1;
            smallerBounds.yMax += -1;
            smallerBounds.zMax += -1;

            if (smallerBounds.x == -1 && smallerBounds.y == -1 && smallerBounds.z == -1)
               yield break;

            FaceView face = default;
            Vector3Int coord = default;

            foreach (var p in smallerBounds.allPositionsWithin)
            {
                coord.x = p.x - x;
                coord.y = p.y - y;
                coord.z = p.z - z;

                var voxel = voxels[coord.x, coord.y, coord.z];
                var voxelR = voxels[coord.x + 1, coord.y, coord.z];
                var voxelU = voxels[coord.x, coord.y + 1, coord.z];
                var voxelF = voxels[coord.x, coord.y, coord.z + 1];

                bool plain = voxel.type == VoxelType.PLAIN;

                if (voxel.type != voxelR.type)
                {
                    face.Set(plain, voxel, p, voxelR, p + Vector3Int.right);
                    yield return face;
                }

                if (voxel.type != voxelU.type)
                {
                    face.Set(plain, voxel, p, voxelU, p + Vector3Int.up);
                    yield return face;
                }

                if (voxel.type != voxelF.type)
                {
                    face.Set(plain, voxel, p, voxelF, p + Vector3Int.forward);
                    yield return face;
                }
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

        public static BoundsInt BoundsUnion(BoundsInt A, BoundsInt B)
        {
            return new BoundsInt(
                Mathf.Min(A.xMin, B.xMin),
                Mathf.Min(A.yMin, B.yMin),
                Mathf.Min(A.zMin, B.zMin),
                Mathf.Max(A.xMax, B.xMax),
                Mathf.Max(A.yMax, B.yMax),
                Mathf.Max(A.zMax, B.zMax));
        }

        public static BoundsInt BoundsUnion(IEnumerable<GameObject> gameObjects) => BoundsUnion(gameObjects.Select(gameObject => ToBounds(gameObject)));
        public static BoundsInt BoundsUnion(IEnumerable<Component> components) => BoundsUnion(components.Select(component => ToBounds(component)));
        public static BoundsInt BoundsUnion(IEnumerable<Transform> transforms) => BoundsUnion(transforms.Select(transform => ToBounds(transform)));
        public static BoundsInt BoundsUnion(IEnumerable<BoundsInt> allBounds)
        {
            var total = new BoundsInt(0, 0, 0, -1, -1, -1);

            BoundsInt bounds;

            var iter = allBounds.GetEnumerator();

            if (iter.MoveNext())
            {
                // First time, set "total".
                total = iter.Current;
            }

            while (iter.MoveNext())
            {
                // Next times, compute.
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

        public static bool BoundsIntersection(BoundsInt A, BoundsInt B, out BoundsInt intersection)
        {
            int xMin = Mathf.Max(A.xMin, B.xMin);
            int yMin = Mathf.Max(A.yMin, B.yMin);
            int zMin = Mathf.Max(A.zMin, B.zMin);
            int xMax = Mathf.Min(A.xMax, B.xMax);
            int yMax = Mathf.Min(A.yMax, B.yMax);
            int zMax = Mathf.Min(A.zMax, B.zMax);

            bool intersects = (xMin <= xMax
                && yMin <= yMax
                && zMin <= zMax);

            intersection = intersects
                ? new BoundsInt(xMin, yMin, zMin, xMax - xMin, yMax - yMin, zMax - zMin)
                : new BoundsInt(0, 0, 0, -1, -1, -1);

            return intersects;
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