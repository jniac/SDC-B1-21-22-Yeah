using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardJumper : MonoBehaviour
{
    public Vector3 jump = new Vector3(0f, 6f, 0f);
    public LayerMask mask = ~0;

    public bool useAnchor = false;
    public Vector3 anchor = new Vector3(0f, 0.5f, 0f);

    public bool lockGizmos = false;

    bool Match(int layer) => (mask & (1 << layer)) != 0;

    void OnTriggerEnter(Collider collider)
    {
        Rigidbody body = collider.attachedRigidbody;

        if (body != null)
        {
            if (Match(body.gameObject.layer) == false)
                return;

            if (useAnchor)
                body.position = transform.TransformPoint(anchor);

            var (v, t) = Utils.GetJumpVelocityAndApogee(jump);
            body.velocity = v;

            body.BroadcastMessage("RemoveControls", t * 0.5f, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnDrawGizmos()
    {
        if (lockGizmos)
            OnDrawGizmosSelected();
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
