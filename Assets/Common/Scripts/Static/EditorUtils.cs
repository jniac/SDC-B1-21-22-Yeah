using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorUtils
{
    public static void ChangeCheck(System.Action action, System.Action hasChangeCallback)
    {
        EditorGUI.BeginChangeCheck();
        action();
        if (EditorGUI.EndChangeCheck())
            hasChangeCallback();
    }

    public static void SetDirty(IEnumerable<Object> objects)
    {
        foreach(var obj in objects)
            EditorUtility.SetDirty(obj);
    }
}
