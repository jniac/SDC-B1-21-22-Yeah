using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class StaticBlockSnapping : MonoBehaviour
{
    void Snap()
    {
        Vector3 size = transform.localScale;
        size.x = Mathf.Round(size.x);
        size.y = Mathf.Round(size.y);
        size.z = Mathf.Round(size.z);
        transform.localScale = size;

        Vector3 position = transform.position;
        position += -size / 2f;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        position.z = Mathf.Round(position.z);
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
