using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EditingBlockSnapping : MonoBehaviour
{
    public enum SnapStep
    {
        Unit,
        Half,
        Quarter,
    }

    public static float SnapModeToScalar(SnapStep mode)
    {
        switch (mode)
        {
            default:
            case SnapStep.Unit:
                return 1f;

            case SnapStep.Half:
                return 1f / 2f;

            case SnapStep.Quarter:
                return 1f / 4f;
        }
    }

    public static void ApplySnapXYZ(Transform transform, SnapStep mode)
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

    public SnapStep snapStep = SnapStep.Unit;
    public bool drawGizmos = false;
    public Color gizmosColor = Color.yellow;

    void Update()
    {
#if UNITY_EDITOR   
        if (Application.isPlaying == false)
            ApplySnapXYZ(transform, snapStep);
#endif
    }

    void OnEnable()
    {
        // Hm...
        gameObject.tag = "Block";
    }

    void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.color = gizmosColor;
            GizmosUtils.WithAlpha(0.25f, () => 
                Gizmos.DrawCube(transform.position, transform.localScale));
        }
    }
}
