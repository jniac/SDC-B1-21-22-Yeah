using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EditingBlockSnapping : MonoBehaviour
{
    public enum SnapMode
    {
        Unit,
        Half,
        Quarter,
    }

    public static float SnapModeToScalar(SnapMode mode)
    {
        switch (mode)
        {
            default:
            case SnapMode.Unit:
                return 1f;

            case SnapMode.Half:
                return 1f / 2f;

            case SnapMode.Quarter:
                return 1f / 4f;
        }
    }

    public SnapMode mode = SnapMode.Unit;

    void Snap()
    {
        var s = SnapModeToScalar(mode);

        Vector3 size = transform.localScale;
        size.x = Mathf.Abs(Mathf.Round(size.x / s) * s);
        size.y = Mathf.Abs(Mathf.Round(size.y / s) * s);
        size.z = Mathf.Abs(Mathf.Round(size.z / s) * s);
        transform.localScale = size;

        Vector3 position = transform.position;
        position += -size / 2f;
        position.x = Mathf.Round(position.x / s) * s;
        position.y = Mathf.Round(position.y / s) * s;
        position.z = Mathf.Round(position.z / s) * s;
        transform.position = position + size / 2f;
    }

    void Update()
    {
#if UNITY_EDITOR   
        if (Application.isPlaying == false)
            Snap();
#endif
    }
}
