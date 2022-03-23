using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardJumper : MonoBehaviour
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

            var (v, t) = Utils.GetJumpVelocityAndApogee(jump);
            body.velocity = v;

            body.BroadcastMessage("RemoveControls", t, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        float dt = 1f / 40f;
        var (v, t) = Utils.GetJumpVelocityAndApogee(jump);
        int count = Mathf.CeilToInt(t / dt);
        var p = transform.TransformPoint(anchor);
        for (int i = 1; i <= count; i++)
        {
            p += v * dt;
            v += Physics.gravity * dt;
            Gizmos.DrawSphere(p, 0.1f);
            Gizmos.DrawWireCube(p, Vector3.one);
        }
    }
}
