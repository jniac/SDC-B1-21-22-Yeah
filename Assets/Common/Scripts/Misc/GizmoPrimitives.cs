using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmoPrimitives
{
    public enum Orientation
    {
        XY,
        XZ,
        YZ,
    }

    public static Vector3 GetOrientationVector(Orientation orientation)
    {
        if (orientation == Orientation.XY)
            return Vector3.forward;

        else if (orientation == Orientation.XZ)
            return Vector3.up;

        else
            return Vector3.left;
    }

    public static IEnumerable<Vector3> Arc(
        float radius = 1f,
        int subdivisions = 24,
        Vector3 center = default,
        Orientation orientation = Orientation.XY,
        float angle = 2f * Mathf.PI,
        bool includeEnd = true
    )
    {
        int max = includeEnd ? subdivisions + 1 : subdivisions;
        for (int i = 0; i < max; i++)
        {
            var t = (float)i / subdivisions;
            var a = t * angle;
            var x = radius * Mathf.Cos(a);
            var y = radius * Mathf.Sin(a);

            if (orientation == Orientation.XY)
                yield return center + new Vector3(x, y, 0);

            else if (orientation == Orientation.XZ)
                yield return center + new Vector3(x, 0, y);

            else
                yield return center + new Vector3(0, x, y);
        }
    }

    public static void DrawCircle(
        float radius = 1f,
        int subdivisions = 24,
        Vector3 center = default,
        Orientation orientation = Orientation.XY
    )
    {
        var points = Arc(
            radius, 
            subdivisions,
            center, 
            orientation).GetEnumerator();
        
        points.MoveNext();
        var p = points.Current;

        while (points.MoveNext())
        {
            var q = points.Current;
            Gizmos.DrawLine(p, q);
            p = q;
        }
    }

    public static void DrawCylinder(
        float radius = 1f,
        float height = 2f,
        int circleSubdivisions = 32,
        int heightSubdivision = 3,
        int ringSubdivisions = 8,
        Vector3 center = default,
        Orientation orientation = Orientation.XY
    )
    {
        var delta = GetOrientationVector(orientation) * height;

        for (int i = 0; i < heightSubdivision; i++)
        {
            float t = (float) i / (heightSubdivision - 1) - 0.5f;
            var p = center + delta * t;
            DrawCircle(radius, circleSubdivisions, p, orientation);
        }

        foreach (var p in Arc(radius, ringSubdivisions, center, orientation))
            Gizmos.DrawLine(p - delta * 0.5f, p + delta * 0.5f);
    }
}
