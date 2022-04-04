using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CubeGroundDetection))]
public class CubeMove : MonoBehaviour
{
    public enum InputMode
    {
        CameraY,
        CameraYQuarter,
        World,
    }

    public InputMode inputMode = InputMode.CameraYQuarter;

    public float speed = 5f;

    [Tooltip("Combien de temps (en secondes) après avoir quitté le sol peut-on encore se déplacer comme si on était au sol ?")]
    public float airTimeTolerance = 0.3f;
    [Tooltip("À quelle distance après avoir quitté le sol peut-on encore se déplacer comme si on était au sol ?")]
    public float airDistanceTolerance = 0.85f;

    [Range(0, 1), Tooltip("Command le ralentissement du cube lorsque le joueur ne demande pas de mouvement.\n"
        + "\n0.15 signifie : Au bout d'une seconde, il ne reste plus que 15% de la vitesse initiale.")]
    public float groundIdleDrag = 0.15f;
    Vector3 groundIdleScale3;

    [Range(0, 1), Tooltip("Commande la quantité de \"contrôle\" dans les airs.\n"
        + "\n0 : Aucun contrôle, le joueur ne peut rien faire dans les airs."
        + "\n1 : Contrôle total, le joueur se déplace dans les airs avec la même efficacité qu'au sol.")]
    public float airControl = 0.2f;

    [Tooltip("Seuil au delà duquel le GameObject est détruit.")]
    public float destroyUnderThreshold = -100f;

    public PhysicMaterial rubber, ice;

    [Range(-1, 1)]
    public float overrideInputX = 0;
    [Range(-1, 1)]
    public float overrideInputY = 0;

    Rigidbody body;
    Collider[] colliders;
    CubeGroundDetection groundDetection;

    float yVelocity = 0f;

    Vector2 input = new Vector2();
    public Vector3 InputVector3 => new Vector3(input.x, 0f, input.y);
    Vector3 inputVelocity = new Vector3();
    public Vector3 InputVelocity => inputVelocity;

    public Vector3 Direction { get; private set; } = new Vector3();
    public Vector3 DirectionRight { get; private set; } = new Vector3();

    public float NoControlsUntil { get; private set; } = 0f;
    public float ControlsCoeff { get; private set; } = 1f;

    void Start()
    {
        groundDetection = GetComponent<CubeGroundDetection>();
        body = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>()
            .Where(c => c.isTrigger == false)
            .ToArray();

        float scale = Mathf.Pow(groundIdleDrag, Time.fixedDeltaTime);
        groundIdleScale3 = new Vector3(scale, 1f, scale);
    }

    public bool GetOnGround()
    {
        return
            groundDetection.airDelta.magnitude < airDistanceTolerance
            && groundDetection.airTime < airTimeTolerance;
    }

    void ComputeInput()
    {
        float x = overrideInputX != 0 ? overrideInputX : Input.GetAxis("Horizontal");
        float y = overrideInputY != 0 ? overrideInputY : Input.GetAxis("Vertical");

        switch (inputMode)
        {
            // Use rotation Y (only) from main camera to transform the inputs.
            case InputMode.CameraY:
            {
                float ry = Camera.main.transform.rotation.eulerAngles.y;
                Vector3 v = Quaternion.Euler(0f, ry, 0f) * new Vector3(x, 0f, y);
                x = v.x;
                y = v.z;
                break;
            }

            // Same as above, but clamped to X/Z axis.
            case InputMode.CameraYQuarter:
            {
                float ry = Camera.main.transform.rotation.eulerAngles.y;
                ry = Mathf.Round(ry / 90f) * 90f;
                Vector3 v = Quaternion.Euler(0f, ry, 0f) * new Vector3(x, 0f, y);
                x = v.x;
                y = v.z;
                break;
            }
        }

        input.x = x;
        input.y = y;
    }

    void Move()
    {
        if (body.position.y < destroyUnderThreshold)
        {
            Destroy(gameObject);
            return;
        }

        ComputeInput();

        ControlsCoeff = Mathf.Lerp(0f, 1f, (Time.time - NoControlsUntil) / 0.3f);
        float inputInfluence = Mathf.Clamp01(Mathf.Abs(input.x) + Mathf.Abs(input.y))
            * ControlsCoeff;

        // inputVelocity = body.velocity;

        bool onGround = GetOnGround();

        // Angular velocity on ground only!
        if (onGround)
        {
            Vector3 angularVelocity = body.angularVelocity;
            angularVelocity.z = -90 * inputVelocity.x;
            angularVelocity.x = 90 * inputVelocity.z;
            body.angularVelocity = Vector3.Lerp(body.angularVelocity, angularVelocity, inputInfluence * 0.1f);
        }

        inputVelocity.x = input.x * speed;
        inputVelocity.z = input.y * speed;
        Direction = inputVelocity.normalized;
        DirectionRight = Vector3.Cross(Vector3.up, Direction);

        // Ground drag, here is the fine tuning that prevent the cube from moving 
        // too fast when the player released any movement inputs. It allows to move
        // from on cell to its neighbors without going any further.
        // Occurs only on ground.
        Vector3 lowVelocity = onGround
            ? Vector3.Scale(body.velocity, groundIdleScale3)
            : body.velocity;

        float control = (onGround
            ? inputInfluence
            : inputInfluence * airControl);

        Vector3 newVelocity = Vector3.Lerp(lowVelocity, inputVelocity, control);

        // Gravity hack:
        // On ground use current "y" velocity
        // Otherwise use an "independant" velocity (which is not affected by walls)
        yVelocity = onGround
            ? body.velocity.y
            : yVelocity + Physics.gravity.y * Time.fixedDeltaTime;

        newVelocity.y = yVelocity;
        body.velocity = newVelocity;

        // "Ascending" is key key concept here: when ascending -> no rubber.
        bool ascending = yVelocity > 0.5f;
        PhysicMaterial physicMaterial = (onGround && ascending == false) ? rubber : ice;
        foreach (var collider in colliders)
            collider.material = physicMaterial;
    }

    public void Jump(float jumpHeight)
    {
        float margin = 0.25f;
        var info = Utils.GetJumpInfo(jumpHeight + margin);
        yVelocity = info.velocity;

        Vector3 velocity = body.velocity;
        velocity.y = info.velocity;
        body.velocity = velocity;
    }

    public void RemoveControls(float duration)
    {
        NoControlsUntil = Mathf.Max(NoControlsUntil, Time.time + duration);
    }

    void OnSwitchCamera()
    {
        // Not necessary any longer?
        // RemoveControls(.25f);
    }

    void FixedUpdate()
    {
        Move();
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        GizmosUtils.DrawArrow(transform.position, inputVelocity);
    }

    [Tooltip("Show debug info on screen.")]
    public bool debug = false;
    void OnGUI()
    {
        if (debug)
        {
            var style = new GUIStyle();
            style.fontSize = 32;
            style.normal.textColor = new Color(0.6f, 1.0f, 0.8f);
            Rigidbody body = GetComponent<Rigidbody>();
            GUI.Label(new Rect(10, 10, 150, 100), $"ground: {GetOnGround()} inputHV: ({input.x:F2}, {input.y:F2}) {body.velocity.magnitude:F2}", style);
        }
    }

#endif
}
