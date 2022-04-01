using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform DeepFind(this Transform parent, string name)
    {
        if (parent == null)
            throw new System.Exception("\"parent\" is null! Can't perform FindDeepChild.");

        Queue<Transform> queue = new Queue<Transform>();

        queue.Enqueue(parent);

        while (queue.Count > 0)
        {
            var child = queue.Dequeue();

            if (child.name == name)
                return child;

            foreach (Transform t in child)
                queue.Enqueue(t);
        }

        return null;
    }
}
