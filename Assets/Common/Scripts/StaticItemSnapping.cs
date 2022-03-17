#if UNITY_EDITOR   
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class StaticItemSnapping : MonoBehaviour
{
    public LayerMask groundMask = 1;
    public float yOver = 0f;
    public bool snapScaleXZ = true;

    float GetY(float defaultY)
    {
        var hits = Physics.RaycastAll(transform.position + Vector3.up * 10f, Vector3.down, 100f, groundMask);

        float y = float.NegativeInfinity;
        foreach (var hit in hits)
        {
            // Ignore prefabs.
            bool isPrefabInstance = PrefabUtility.GetCorrespondingObjectFromOriginalSource(hit.collider.gameObject) != null;
            if (isPrefabInstance)
                continue;

            // Ignore static items.
            bool isStaticItemToo = hit.collider.gameObject.GetComponent<StaticItemSnapping>() != null;
            if (isStaticItemToo)
                continue;

            // Ignore triggers.
            if (hit.collider.isTrigger)
                continue;

            // Ignore self.
            if (hit.collider.gameObject == gameObject)
                continue;

            float currentY = hit.point.y + yOver;
            if (currentY > y)
                y = currentY;
        }

        // Ignore invalid y value.
        if (y == float.NegativeInfinity)
            return defaultY;

        return y;
    }

    void SnapScaleXZ()
    {
        float x = Mathf.Round(transform.localScale.x);
        float y = transform.localScale.y;
        float z = Mathf.Round(transform.localScale.z);
        transform.localScale = new Vector3(x, y, z);
    }

    void Snap()
    {
        if (snapScaleXZ)
            SnapScaleXZ();

        Vector3 position = transform.position;
        position += -transform.localScale / 2f;
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);
        position += transform.localScale / 2f;

        position.y = GetY(position.y);

        transform.position = position;
    }

    void Update()
    {
        if (Application.isPlaying == false)
            Snap();
    }
}
#endif
