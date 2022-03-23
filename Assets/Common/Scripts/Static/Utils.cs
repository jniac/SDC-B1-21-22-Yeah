using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static T RequireComponent<T>(GameObject target) where T : Component
    {
        var component = target.GetComponent<T>();

        if (component == null) {
            component = target.AddComponent<T>();
        }

        return component;
    }

    public static IEnumerable<Transform> AllChildren(Transform target, bool includeTarget = true)
    {
        if (includeTarget)
            yield return target;

        var queue = new Queue<Transform>();
        queue.Enqueue(target);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (Transform child in current)
            {
                queue.Enqueue(child);
                yield return child;
            }
        }
    }

    /// <summary>
    /// Works approximately since Unity do not integrate gravity over frame, but simply does `v += g * dt`.
    /// </summary>
    public static (float velocity, float apogee) GetJumpInfo(float height)
    {
        float g = Physics.gravity.magnitude;
        float t = Mathf.Pow(2f * height / g, 1f / 4f);
        float v = Mathf.Sqrt(height * 2f * g);
        return (v, t);
    }

    /// <summary>
    /// Assumes that Physics.gravity is (0, g, 0) where g <= 0.
    /// </summary>
    public static (Vector3 velocity, float apogee) GetJumpVelocityAndApogee(Vector3 jump)
    {
        if (jump.y < 0)
            return (Vector3.zero, 0f);
        
        var (y, t) = GetJumpInfo(jump.y);
        float x = jump.x / t;
        float z = jump.z / t;
        
        return (new Vector3(x, y, z), t);
    }
}
