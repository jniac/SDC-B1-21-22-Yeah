using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_HardJumper : MonoBehaviour
{
    public Vector3 jump = new Vector3(0f, 5f, 0f);

    public bool useAnchor = true;
    public Vector3 anchor = new Vector3(0f, 0.5f, 0f);

    void OnTriggerEnter(Collider collider)
    {
        Rigidbody body = collider.attachedRigidbody;

        if (body != null)
        {
            if (useAnchor)
                body.position = transform.TransformPoint(anchor);

            body.velocity = Utils.GetJumpVelocity(jump);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        float dt = 1f / 40f;
        var info = Utils.GetJumpInfo(jump.y);
        int count = Mathf.CeilToInt(info.apogee / dt);
        var p = transform.TransformPoint(anchor);
        var v = Utils.GetJumpVelocity(jump);
        for (int i = 1; i <= count; i++)
        {
            p += v * dt;
            v += Physics.gravity * dt;
            Gizmos.DrawSphere(p, 0.1f);
            Gizmos.DrawWireCube(p, Vector3.one);
        }
    }
}
