using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_Attractor : MonoBehaviour
{
    public LayerMask mask = 1 << 6; // "Player"

    public Vector3 anchor = new Vector3(0f, 0.5f, 0f);

    bool Match(int layer) => (mask & (1 << layer)) != 0;
    bool Match(GameObject go) => go != null && Match(go.layer);

    void OnTriggerEnter(Collider other)
    {
        if (Match(other.attachedRigidbody?.gameObject))
        {
            other.attachedRigidbody.position = transform.TransformPoint(anchor);
            other.attachedRigidbody.velocity *= 0.25f;
            other.attachedRigidbody.gameObject.BroadcastMessage("RemoveControls", 0.25f);
        }
    }
}
