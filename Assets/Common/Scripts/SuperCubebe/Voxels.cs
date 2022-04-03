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

    public enum Axis
    {
        X_POSITIVE = 0,
        X_NEGATIVE = 1,
        Y_POSITIVE = 2,
        Y_NEGATIVE = 3,
        Z_POSITIVE = 4,
        Z_NEGATIVE = 5,
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
        public Axis axis;
        public bool voxel1IsPlain;
        public Voxel voxel1, voxel2;
        public Vector3Int position1, position2;
        public Vector3 Center => (Vector3)(position2 + position1) / 2f + Vector3.one / 2f;
        public Vector3 Normal => voxel1IsPlain ? (Vector3)(position2 - position1) : (Vector3)(position1 - position2);
        public Vector3 Size => Vector3.one - (position2 - position1);

        public (Vector3Int, Vector3Int, Vector3Int, Vector3Int) Vertices
        {
            get 
            {
                // Elegant, but unsorted, so not useful.
                // var p = position2;
                // var d = position2 - position1;
                // var s = Vector3Int.one - d;
                // return (
                //     p,
                //     p + new Vector3Int(s.x, s.y, 0),
                //     p + new Vector3Int(s.x, 0, s.z),
                //     p + new Vector3Int(0, s.y, s.z)
                // );

                var p = position2;
                switch (axis)
                {
                    default:
                    case Axis.X_POSITIVE: return (
                        p, 
                        p + new Vector3Int(0, 1, 0),
                        p + new Vector3Int(0, 1, 1),
                        p + new Vector3Int(0, 0, 1)
                    );
                    case Axis.X_NEGATIVE: return (
                        p, 
                        p + new Vector3Int(0, 0, 1),
                        p + new Vector3Int(0, 1, 1),
                        p + new Vector3Int(0, 1, 0)
                    );
                    case Axis.Y_POSITIVE: return (
                        p, 
                        p + new Vector3Int(0, 0, 1),
                        p + new Vector3Int(1, 0, 1),
                        p + new Vector3Int(1, 0, 0)
                    );
                    case Axis.Y_NEGATIVE: return (
                        p, 
                        p + new Vector3Int(1, 0, 0),
                        p + new Vector3Int(1, 0, 1),
                        p + new Vector3Int(0, 0, 1)
                    );
                    case Axis.Z_POSITIVE: return (
                        p, 
                        p + new Vector3Int(1, 0, 0),
                        p + new Vector3Int(1, 1, 0),
                        p + new Vector3Int(0, 1, 0)
                    );
                    case Axis.Z_NEGATIVE: return (
                        p, 
                        p + new Vector3Int(0, 1, 0),
                        p + new Vector3Int(1, 1, 0),
                        p + new Vector3Int(1, 0, 0)
                    );
                }
            }
        }

        public void Set(Axis axis, bool voxel1IsPlain, Voxel voxel1, Vector3Int position1, Voxel voxel2, Vector3Int position2)
        {
            this.axis = axis;
            this.voxel1IsPlain = voxel1IsPlain;
            this.voxel1 = voxel1;
            this.position1 = position1;
            this.voxel2 = voxel2;
            this.position2 = position2;
        }
    }

    public class VoxelWorld
    {
        public static VoxelWorld New(IEnumerable<GameObject> items) => new VoxelWorld(items.Select(item => Utils.ToBounds(item)).ToArray());
        public static VoxelWorld New(IEnumerable<Component> items) => new VoxelWorld(items.Select(item => Utils.ToBounds(item)).ToArray());
        public static VoxelWorld New(IEnumerable<Transform> items) => new VoxelWorld(items.Select(item => Utils.ToBounds(item)).ToArray());

        public static VoxelWorld FromChildren(GameObject go)
        {
            var children = new List<GameObject>();
            foreach (Transform child in go.transform)
                children.Add(child.gameObject);
            return New(children);
        }

        public BoundsInt bounds;
        public Voxel[,,] voxels;
        public readonly int x, y, z;
        public readonly int sizeX, sizeY, sizeZ;

        public VoxelWorld(BoundsInt[] allBounds)
        {
            bounds = Utils.BoundsUnion(allBounds);
            
            x = bounds.xMin;
            y = bounds.yMin;
            z = bounds.zMin;
            sizeX = bounds.size.x;
            sizeY = bounds.size.y;
            sizeZ = bounds.size.z;

            if (sizeX == -1 || sizeY == -1 || sizeZ == -1)
            {
                voxels = new Voxel[0, 0, 0];
                Debug.Log("Null world (no Voxel)!");
                return;
            }

            voxels = new Voxel[sizeX, sizeY, sizeZ];

            foreach (var currentBounds in allBounds)
            {
                foreach (var p in currentBounds.allPositionsWithin)
                {
                    voxels[p.x - x, p.y - y, p.z - z].type = VoxelType.PLAIN;
                }
            }
        }

        public IEnumerable<VoxelView> VoxelViews()
        {
            VoxelView voxel = default;

            foreach (var p in bounds.allPositionsWithin)
            {
                voxel.position = p;
                voxel.voxel = voxels[p.x - x, p.y - y, p.z - z];
                yield return voxel;
            }
        }

        public IEnumerable<FaceView> FaceViews(bool includeOutsideFace = true)
        {
            // OPTIM: There is a lot of branches here. 
            // Some of them are almost always true: 
            // `cx + 1 < sizeX` is false for only one cube's slice. 
            // An optimization may consist here to perform 4 loops:
            // - One for bounds - 1
            // - One for x == sizeX - 1
            // - One for y == sizeY - 1
            // - One for z == sizeZ - 1
            // But it's for another time!

            var dX = new Vector3Int(1, 0, 0);
            var dY = new Vector3Int(0, 1, 0);
            var dZ = new Vector3Int(0, 0, 1);

            FaceView face = default;
            int cX, cY, cZ;

            var voidVoxel = new Voxel { type = VoxelType.VOID };

            foreach (var p in bounds.allPositionsWithin)
            {
                cX = p.x - x;
                cY = p.y - y;
                cZ = p.z - z;

                var voxel = voxels[cX, cY, cZ];
                bool plain = voxel.type == VoxelType.PLAIN;

                if (includeOutsideFace && voxel.type == VoxelType.PLAIN)
                {
                    // X:
                    if (cX == 0)
                    {
                        var axis = Axis.X_NEGATIVE;
                        face.Set(axis, false, voidVoxel, p - dX, voxel, p);
                        yield return face;
                    }

                    if (cX == sizeX - 1)
                    {
                        var axis = Axis.X_POSITIVE;
                        face.Set(axis, true, voxel, p, voidVoxel, p + dX);
                        yield return face;
                    }

                    // Y:
                    if (cY == 0)
                    {
                        var axis = Axis.Y_NEGATIVE;
                        face.Set(axis, false, voidVoxel, p - dY, voxel, p);
                        yield return face;
                    }

                    if (cY == sizeY - 1)
                    {
                        var axis = Axis.Y_POSITIVE;
                        face.Set(axis, true, voxel, p, voidVoxel, p + dY);
                        yield return face;
                    }

                    // Z:
                    if (cZ == 0)
                    {
                        var axis = Axis.Z_NEGATIVE;
                        face.Set(axis, false, voidVoxel, p - dZ, voxel, p);
                        yield return face;
                    }

                    if (cZ == sizeZ - 1)
                    {
                        var axis = Axis.Z_POSITIVE;
                        face.Set(axis, true, voxel, p, voidVoxel, p + dZ);
                        yield return face;
                    }
                }
                
                if (cX < sizeX - 1)
                {
                    var voxelX = voxels[cX + 1, cY, cZ];
                    if (voxel.type != voxelX.type)
                    {
                        var axis = plain ? Axis.X_POSITIVE : Axis.X_NEGATIVE;
                        face.Set(axis, plain, voxel, p, voxelX, p + dX);
                        yield return face;
                    }
                }

                if (cY < sizeY - 1)
                {
                    var voxelY = voxels[cX, cY + 1, cZ];
                    if (voxel.type != voxelY.type)
                    {
                        var axis = plain ? Axis.Y_POSITIVE : Axis.Y_NEGATIVE;
                        face.Set(axis, plain, voxel, p, voxelY, p + dY);
                        yield return face;
                    }
                }

                if (cZ < sizeZ - 1)
                {
                    var voxelZ = voxels[cX, cY, cZ + 1];
                    if (voxel.type != voxelZ.type)
                    {
                        var axis = plain ? Axis.Z_POSITIVE : Axis.Z_NEGATIVE;
                        face.Set(axis, plain, voxel, p, voxelZ, p + dZ);
                        yield return face;
                    }
                }
            }
        }
    }

    public class Utils
    {
        public static Vector3 ToVector(Axis axis)
        {
            switch (axis)
            {
                default:
                case Axis.X_POSITIVE: return Vector3.right;
                case Axis.X_NEGATIVE: return Vector3.left;
                case Axis.Y_POSITIVE: return Vector3.up;
                case Axis.Y_NEGATIVE: return Vector3.down;
                case Axis.Z_POSITIVE: return Vector3.forward;
                case Axis.Z_NEGATIVE: return Vector3.back;
            }
        }

        public static Axis ToAxis(Vector3 v)
        {
            float ax = Mathf.Abs(v.x);
            float ay = Mathf.Abs(v.y);
            float az = Mathf.Abs(v.z);
            if (ax > ay)
            {
                if (ax > az)
                    return v.x > 0f ? Axis.X_POSITIVE : Axis.X_NEGATIVE;
                else
                    return v.z > 0f ? Axis.Z_POSITIVE : Axis.Z_NEGATIVE;
            }
            else
            {
                if (ay > az)
                    return v.y > 0f ? Axis.Y_POSITIVE : Axis.Y_NEGATIVE;
                else
                    return v.z > 0f ? Axis.Z_POSITIVE : Axis.Z_NEGATIVE;
            }
        }

        public static BoundsInt ToBounds(GameObject gameObject, bool useLocalPosition = true) => ToBounds(gameObject.transform, useLocalPosition);
        public static BoundsInt ToBounds(Component component, bool useLocalPosition = true) => ToBounds(component.transform, useLocalPosition);
        public static BoundsInt ToBounds(Transform transform, bool useLocalPosition = true)
        {
            var p = useLocalPosition ? transform.localPosition : transform.position;
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

        public static void Inflate(ref BoundsInt bounds, int offset)
        {
            bounds.x += -offset;
            bounds.y += -offset;
            bounds.z += -offset;
            bounds.xMax += offset * 2;
            bounds.yMax += offset * 2;
            bounds.zMax += offset * 2;
        }
        public static BoundsInt Inflate(BoundsInt bounds, int offset)
        {
            var copy = bounds;
            Inflate(ref copy, offset);
            return copy;
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

        public static IEnumerable<Vector3Int> PositionsInBounds(int x, int y, int z, int xMax, int yMax, int zMax)
        {
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