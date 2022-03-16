using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kevanakin_HardJumper : MonoBehaviour
{
    public float jumpForce = 10f;

    void OnTriggerEnter(Collider collider)
    {
        // Debug.Break();
        Rigidbody body = collider.attachedRigidbody;
        if (body != null)
        {
            body.velocity = new Vector3 (0f,jumpForce,0f);
        }
    }
}
