using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class StaticItemSnapping : MonoBehaviour
{
    public LayerMask groundMask = 1;
    public float yOver = 0f;

    float GetY(float defaultY)
    {
        var hits = Physics.RaycastAll(transform.position + Vector3.up * 10f, Vector3.down, 100f, groundMask);

        float y = float.NegativeInfinity;
        foreach (var hit in hits)
        {
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

    void Snap()
    {
        Vector3 position = transform.position;
        position += -Vector3.one / 2f;
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);
        position += Vector3.one / 2f;
        
        position.y = GetY(position.y);

        transform.position = position;
    }

    void Update()
    {
#if UNITY_EDITOR   
        if (Application.isPlaying == false)
            Snap();
#endif
    }
}
