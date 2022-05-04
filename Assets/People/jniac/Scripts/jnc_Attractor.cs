using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_Attractor : MonoBehaviour
{
    public LayerMask mask = 1 << 6; // "Player"

    public Vector3 anchor = new Vector3(0f, 0.5f, 0f);

    bool Match(int layer) => (mask & (1 << layer)) != 0;

    void OnTriggerEnter(Collider other)
    {
        var body = other.attachedRigidbody;
        if (body != null && Match(body.gameObject.layer))
        {
            body.position = transform.TransformPoint(anchor);
            body.rotation = Quaternion.identity;
            body.velocity = Vector3.zero;
            body.gameObject.BroadcastMessage("RemoveControls", 0.25f);
        }
    }
}
