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
    public float airDistanceTolerance = 0.5f;

    enum JumpRequestStatus
    {
        TooLate,
        TooFar,
        DestroyerAbove,
        Ok,
    }

    JumpRequestStatus CanJump()
    {
        var groundDetection = GetComponent<CubeGroundDetection>();

        if (groundDetection.airTime > airTimeTolerance)
            return JumpRequestStatus.TooLate;

        if (groundDetection.airDelta.magnitude > airDistanceTolerance)
            return JumpRequestStatus.TooFar;

        bool destoyerAboveGround = groundDetection.aboveGroundTriggers
            .Find(collider => collider.gameObject.GetComponent<Destroyer>() != null);
        if (destoyerAboveGround)
            return JumpRequestStatus.DestroyerAbove;

        return JumpRequestStatus.Ok;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            var status = CanJump();
   
            if (status == JumpRequestStatus.Ok) 
                GetComponent<CubeMove>().Jump(jumpHeight);
        }
    }

}

