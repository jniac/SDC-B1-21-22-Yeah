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
}
