using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CubeGroundDetection), typeof(CubeMove))]
public class CubeJump : MonoBehaviour
{
    public float jumpHeight = 2f;
    [Tooltip("Combien de temps (en secondes) après avoir quitté le sol peut-on encore sauter ?")]
    public float airTimeTolerance = 0.3f;
    [Tooltip("À quelle distance après avoir quitté le sol peut-on encore sauter ?")]
    public float airDistanceTolerance = 0.85f;

    public int airJumpMax = 0;
    public int extraJumpCount = 0;

    public int JumpCount { get; private set; } = 0;
    public int AirJumpCount { get; private set; } = 0;

    enum JumpRequestStatus
    {
        NoControls,
        NoAirJump,
        NoMoreAirJump,
        DestroyerAboveGround,
        Ok,
    }

    bool onGround, onGroundOld;
    CubeMove move;
    CubeGroundDetection groundDetection;

    public bool GetOnGround()
    {
        return groundDetection.airTime <= airTimeTolerance
            && groundDetection.airDelta.sqrMagnitude <= airDistanceTolerance * airDistanceTolerance;
    }

    public bool ThereIsADestroyerAboveGround()
    {
        return groundDetection.aboveGroundTriggers
            .Find(collider => (
                collider.tag == "Destroyer"
                || collider.gameObject.GetComponent<Destroyer>() != null
            ));
    }

    JumpRequestStatus CanJump()
    {
        if (PlayModeManager.Test(PlayMode.AlwaysJump))
            return JumpRequestStatus.Ok;

        if (move.ControlsCoeff < 0.5f)
            return JumpRequestStatus.NoControls;

        if (onGround == false)
        {
            if (airJumpMax == 0)
                return JumpRequestStatus.NoAirJump;

            if (AirJumpCount >= airJumpMax)
                return JumpRequestStatus.NoMoreAirJump;
        }

        bool destoyerAboveGround = ThereIsADestroyerAboveGround();
        if (destoyerAboveGround)
            return JumpRequestStatus.DestroyerAboveGround;

        return JumpRequestStatus.Ok;
    }

    void Jump()
    {
        move.Jump(jumpHeight);

        if (onGround == false)
            AirJumpCount += 1;

        JumpCount += 1;
    }

    void Start()
    {
        groundDetection = GetComponent<CubeGroundDetection>();
        move = GetComponent<CubeMove>();
    }

    void Update()
    {
        onGroundOld = onGround;
        onGround = GetOnGround();

        if (onGround && onGroundOld == false)
        {
            // Reset counter
            AirJumpCount = 0;
            JumpCount = 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            var status = CanJump();
   
            if (status == JumpRequestStatus.Ok) 
                Jump();
        }
    }
}

