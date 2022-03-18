using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CubeGroundDetection), typeof(CubeMove))]
public class CubeJump : MonoBehaviour
{
    public float jumpSpeed = 10f;
    [Tooltip("Combien de temps (en secondes) après avoir quitté le sol peut-on encore sauter ?")]
    public float airTimeTolerance = 0.3f;

    bool CanJump()
    {
        var groundDetection = GetComponent<CubeGroundDetection>();
        return groundDetection.timeSinceOnGround < airTimeTolerance;
    }

    void Update()
    {

        bool jump = Input.GetButtonDown("Jump") && CanJump();

        if (jump)
        {
            GetComponent<CubeMove>().Jump(jumpSpeed);
        }
    }

}

