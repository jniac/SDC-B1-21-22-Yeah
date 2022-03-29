using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(SphereCollider))]
public class AdjustSphereCollider : MonoBehaviour
{
    public enum AdjustMode
    {
        Cover,
        Contain,
        ContainXZ,
        Average,
    }

    public AdjustMode mode;

    void Adjust()
    {
        var sphere = GetComponent<SphereCollider>();
        float x = transform.localToWorldMatrix.m00;
        float y = transform.localToWorldMatrix.m11;
        float z = transform.localToWorldMatrix.m22;
        float max = Mathf.Max(x, y, z);

        float w;
        switch (mode)
        {
            default:
            case AdjustMode.Contain:
                w = Mathf.Min(x, y, z);
                break;

            case AdjustMode.ContainXZ:
                w = Mathf.Min(x, z);
                break;

            case AdjustMode.Cover:
                w = max;
                break;

            case AdjustMode.Average:
                w = (x + y + z) / 3f;
                break;
        }

        sphere.radius = w / max / 2f;
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
