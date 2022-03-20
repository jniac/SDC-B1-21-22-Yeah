using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_HardJumper : MonoBehaviour
{
    public float jumpForce = 20f;

    void OnTriggerEnter(Collider collider)
    {
        Rigidbody body = collider.attachedRigidbody;
        if (body != null)
        {
            body.velocity = new Vector3(0f, jumpForce, 0f);
        }
    }
}
