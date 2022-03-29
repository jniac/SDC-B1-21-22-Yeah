using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EditingBlockSnapping : MonoBehaviour
{
    public enum SnapXYZMode
    {
        Unit,
        Half,
        Quarter,
    }

    public static float SnapModeToScalar(SnapXYZMode mode)
    {
        switch (mode)
        {
            default:
            case SnapXYZMode.Unit:
                return 1f;

            case SnapXYZMode.Half:
                return 1f / 2f;

            case SnapXYZMode.Quarter:
                return 1f / 4f;
        }
    }

    public static void ApplySnapXYZ(Transform transform, SnapXYZMode mode)
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

    public SnapXYZMode mode = SnapXYZMode.Unit;

    void Update()
    {
#if UNITY_EDITOR   
        if (Application.isPlaying == false)
            ApplySnapXYZ(transform, mode);
#endif
    }
}
