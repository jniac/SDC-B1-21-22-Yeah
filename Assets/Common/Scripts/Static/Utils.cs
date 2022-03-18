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
}
