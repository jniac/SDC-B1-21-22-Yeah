using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StaticItemSnapping : MonoBehaviour
{
    void Snap()
    {
        Vector3 position = transform.position;
        position += -Vector3.one / 2f;
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);
        transform.position = position + Vector3.one / 2f;
    }

    void Update()
    {
        Snap();
    }
}
