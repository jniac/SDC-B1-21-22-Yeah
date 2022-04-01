using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Snapping
{
    public static bool enabled = true;

    public enum SnapStep
    {
        Unit,
        Half,
        Quarter,
    }

    public static float SnapModeToScalar(Snapping.SnapStep mode)
    {
        switch (mode)
        {
            default:
            case Snapping.SnapStep.Unit:
                return 1f;

            case Snapping.SnapStep.Half:
                return 1f / 2f;

            case Snapping.SnapStep.Quarter:
                return 1f / 4f;
        }
    }

    public static void ApplySnapXYZ(Transform transform, Snapping.SnapStep mode)
    {
        var s = SnapModeToScalar(mode);

        Vector3 size = transform.localScale;
        size.x = Mathf.Abs(Mathf.Round(size.x / s) * s);
        size.y = Mathf.Abs(Mathf.Round(size.y / s) * s);
        size.z = Mathf.Abs(Mathf.Round(size.z / s) * s);

        Vector3 position = transform.position;
        position += -transform.localScale / 2f;
        position.x = Mathf.Round(position.x / s) * s;
        position.y = Mathf.Round(position.y / s) * s;
        position.z = Mathf.Round(position.z / s) * s;

        transform.position = position + size / 2f;
        transform.localScale = size;
        transform.rotation = Quaternion.identity;
    }
}
