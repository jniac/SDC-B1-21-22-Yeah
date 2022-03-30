using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(BoxCollider))]
public class AdjustBoxCollider : MonoBehaviour
{
    public Vector3 margin = Vector3.zero;
    public float marginOffset = 0f;

    void Adjust()
    {
        var box = GetComponent<BoxCollider>();
        float x = Mathf.Max(0f, 1f + (margin.x + marginOffset) / transform.localScale.x);
        float y = Mathf.Max(0f, 1f + (margin.y + marginOffset) / transform.localScale.y);
        float z = Mathf.Max(0f, 1f + (margin.z + marginOffset) / transform.localScale.z);
        box.size = new Vector3(x, y, z);
    }

    void Start()
    {
        Adjust();
    }

    void OnValidate()
    {
        Adjust();
    }
}
