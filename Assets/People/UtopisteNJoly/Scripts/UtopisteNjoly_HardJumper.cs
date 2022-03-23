using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtopisteNjoly_HardJumper : MonoBehaviour
{
    public float jumpForce = 15f;

    void OnTriggerEnter(Collider collider)
    {
        // Debug.Break();
        Rigidbody body = collider.attachedRigidbody;

        if (body != null)
        {
            body.velocity = new Vector3(0f, jumpForce, 0f);
        }
    }
}
