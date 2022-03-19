using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CubeGroundDetection))]
public class CubeMove : MonoBehaviour
{
    public float speed = 5f;
    [Range(0, 1), Tooltip("Command le ralentissement du cube lorsque le joueur ne demande pas de mouvement.\n" 
        + "\n0.15 signifie : Au bout d'une seconde, il ne reste plus que 15% de la vitesse initiale.")]
    public float groundIdleDrag = 0.15f;
    [Range(0, 1), Tooltip("Commande la quantité de \"contrôle\" dans les airs.\n"
        + "\n0 : Aucun contrôle, le joueur ne peut rien faire dans les airs."
        + "\n1 : Contrôle total, le joueur se déplace dans les airs avec la même efficacité qu'au sol.")]
    public float airControl = 0.2f;
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

    Vector3 GetGroundIdleScale()
    {
        float scale = Mathf.Pow(groundIdleDrag, Time.fixedDeltaTime);
        return new Vector3(scale, 1f, scale);
    }

    void Move()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        // `controlInfluence`: 0: player is waiting. 1: player is playing.
        float controlInfluence = Mathf.Clamp01(Mathf.Abs(inputH) + Mathf.Abs(inputV));

        Vector3 inputVelocity = body.velocity;

        // Angular velocity on ground only!
        if (groundDetection.onGround)
        {
            Vector3 angularVelocity = body.angularVelocity;
            angularVelocity.z = -90 * inputVelocity.x;
            angularVelocity.x =  90 * inputVelocity.z;
            body.angularVelocity = Vector3.Lerp(body.angularVelocity, angularVelocity, controlInfluence * 0.1f);
        }

        inputVelocity.x = inputH * speed;
        inputVelocity.z = inputV * speed;

        // Ground drag, here is the fine tuning that prevent the cube from moving 
        // too fast when the player released any movement inputs.
        // Occurs only on ground.
        Vector3 lowVelocity = groundDetection.onGround
            ? Vector3.Scale(body.velocity, GetGroundIdleScale())
            : body.velocity;

        float control = groundDetection.onGround 
            ? controlInfluence
            : controlInfluence * airControl;

        Vector3 newVelocity = Vector3.Lerp(lowVelocity, inputVelocity, control);

        // Gravity hack:
        // On ground use current "y" velocity
        // Otherwise use an "independant" velocity (which is not affected by walls)
        yVelocity = groundDetection.onGround 
            ? body.velocity.y 
            : yVelocity + Physics.gravity.y * Time.fixedDeltaTime;

        newVelocity.y = yVelocity;
        body.velocity = newVelocity;
        
        // "Ascending" is key key concept here: when ascending -> no rubber.
        bool ascending = yVelocity > 0.5f;
        PhysicMaterial physicMaterial = (groundDetection.onGround && ascending == false) ? rubber : ice;
        foreach (var collider in colliders)
            collider.material = physicMaterial;
    }

    public void Jump(float yJumpVelocity)
    {
        yVelocity = yJumpVelocity;
        
        Vector3 velocity = body.velocity;
        velocity.y = yJumpVelocity;
        body.velocity = velocity;
    }

    void FixedUpdate()
    {
        Move();
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
