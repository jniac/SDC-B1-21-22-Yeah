using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CubeGroundDetection), typeof(CubeMove))]
public class CubeJump : MonoBehaviour
{
    public float jumpSpeed = 9f;
    [Tooltip("Combien de temps (en secondes) après avoir quitté le sol peut-on encore sauter ?")]
    public float airTimeTolerance = 0.3f;
    [Tooltip("À quelle distance après avoir quitté le sol peut-on encore sauter ?")]
    public float airDistanceTolerance = 0.5f;

    bool CanJump()
    {
        var groundDetection = GetComponent<CubeGroundDetection>();
        bool timeOk = groundDetection.timeSinceOnGround < airTimeTolerance;
        bool distanceOk = groundDetection.deltaSinceOnGround.magnitude < airDistanceTolerance;
        return timeOk && distanceOk;
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

