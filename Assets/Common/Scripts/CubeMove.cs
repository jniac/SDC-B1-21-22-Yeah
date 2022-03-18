using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CubeGroundDetection))]
public class CubeMove : MonoBehaviour
{
    public float speed = 5f;
    public PhysicMaterial rubber, ice;

    Rigidbody body;
    Collider[] colliders;
    CubeGroundDetection groundDetection;

    float yVelocity = 0f;

    void Start()
    {
        groundDetection = GetComponent<CubeGroundDetection>();
        body = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>()
            .Where(c => c.isTrigger == false)
            .ToArray();
    }

    void FixedUpdate()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        // controlInfluence: 0: player is waiting. 1: player is playing.
        float controlInfluence = Mathf.Clamp01(Mathf.Abs(inputH) + Mathf.Abs(inputV));

        Vector3 velocity = body.velocity;

        yVelocity = groundDetection.onGround ? body.velocity.y : yVelocity + Physics.gravity.y * Time.fixedDeltaTime;

        // Angular velocity on ground only!
        // if (groundDetection.onGround)
        // {
        //     Vector3 angularVelocity = body.angularVelocity;
        //     angularVelocity.z = -90 * velocity.x;
        //     angularVelocity.x =  90 * velocity.z;
        //     body.angularVelocity = Vector3.Lerp(body.angularVelocity, angularVelocity, controlInfluence * 0.1f);
        // }

        velocity.x = inputH * speed;
        velocity.y = yVelocity;
        velocity.z = inputV * speed;
        body.velocity = Vector3.Lerp(body.velocity, velocity, controlInfluence);

        PhysicMaterial physicMaterial = groundDetection.onGround ? rubber : ice;
        foreach (var collider in colliders)
            collider.material = physicMaterial;
    }

#if UNITY_EDITOR
    [Tooltip("Show debug info on screen.")]
    public bool debug = false;
    void OnGUI()
    {
        if (debug)
        {
            var style = new GUIStyle();
            style.fontSize = 32;
            style.normal.textColor = new Color(0.6f, 1.0f, 0.8f);
            float inputH = Input.GetAxis("Horizontal");
            float inputV = Input.GetAxis("Vertical");
            Rigidbody body = GetComponent<Rigidbody>();
            GUI.Label(new Rect(10, 10, 150, 100), $"ground: {groundDetection.onGround} inputHV: ({inputH:F2}, {inputV:F2}) {body.velocity.magnitude:F2}", style);
        }
    }
#endif
}
