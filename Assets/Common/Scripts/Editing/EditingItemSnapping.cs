#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class EditingItemSnapping : MonoBehaviour
{
    public enum SnapMode
    {
        Ground,
        XYZ,
    }

    public SnapMode mode = SnapMode.Ground;

    // GROUND MODE
    public LayerMask groundMask = 1;
    public bool snapY = true;
    public float yOver = 0f;
    public bool snapScaleXZ = true;
    public float yRangeWidth = 8f;

    // XYZ Mode
    public Snapping.SnapStep xyzMode = Snapping.SnapStep.Unit;

    IEnumerable<RaycastHit> GetHits()
    {
        // Return hits 
        return Physics.RaycastAll(transform.position + Vector3.up * (yRangeWidth + yOver), Vector3.down, yRangeWidth * 2f, groundMask);
    }

    float GetY(float defaultY)
    {
        var hits = GetHits();

        float d = float.PositiveInfinity;
        float y = defaultY;
        foreach (var hit in hits)
        {
            // Ignore prefabs.
            bool isPrefabInstance = PrefabUtility.GetCorrespondingObjectFromOriginalSource(hit.collider.gameObject) != null;
            if (isPrefabInstance)
                continue;

            // Ignore "editing" items.
            bool isStaticItemToo = hit.collider.gameObject.GetComponent<EditingItemSnapping>() != null;
            if (isStaticItemToo)
                continue;

            // Ignore triggers.
            if (hit.collider.isTrigger)
                continue;

            // Ignore self.
            if (hit.collider.gameObject == gameObject)
                continue;

            float currentD = Mathf.Abs(hit.point.y - transform.position.y);
            if (currentD < d)
            {
                d = currentD;
                y = hit.point.y + yOver;
            }
        }

        return y;
    }

    void SnapScaleXZ()
    {
        float x = Mathf.Round(transform.localScale.x);
        float y = transform.localScale.y;
        float z = Mathf.Round(transform.localScale.z);
        transform.localScale = new Vector3(x, y, z);
    }

    void SnapGround()
    {
        if (snapScaleXZ)
            SnapScaleXZ();

        Vector3 position = transform.position;
        position += -transform.localScale / 2f;
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);
        position += transform.localScale / 2f;

        if (snapY)
            position.y = GetY(position.y);

        if (transform.position != position)
        {
            EditorUtility.SetDirty(gameObject);
            transform.position = position;
        }
    }

    void SnapXYZ()
    {
        Snapping.ApplySnapXYZ(transform, xyzMode);
    }

    void Update()
    {
        if (Application.isPlaying == false && Snapping.enabled)
        {
            switch (mode)
            {
                case SnapMode.Ground:
                    SnapGround();
                    break;

                case SnapMode.XYZ:
                    SnapXYZ();
                    break;
            }
        }
    }

    [CustomEditor(typeof(EditingItemSnapping))]
    class MyEditor : Editor
    {
        EditingItemSnapping Target => target as EditingItemSnapping;
        
        public override void OnInspectorGUI()
        {
            System.Action<string> Draw = prop => EditorGUILayout.PropertyField(serializedObject.FindProperty(prop));

            Draw("mode");

            switch(Target.mode)
            {
                default:
                case SnapMode.Ground:
                    Draw("snapY");
                    Draw("yOver");
                    Draw("snapScaleXZ");
                    Draw("yRangeWidth");
                    break;

                case SnapMode.XYZ:
                    Draw("xyzMode");
                    break;
            }

            Snapping.enabled = EditorGUILayout.Toggle("Snapping Enabled", Snapping.enabled);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
