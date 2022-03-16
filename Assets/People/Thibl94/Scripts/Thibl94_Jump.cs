
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thibl94_Jump : MonoBehaviour
{
    public float JumpVelocity = 10f;
    // Update is called once per frame
    void Update()
    {
        bool canJump = true;
        if (canJump == true)
        {
            bool wouldJump = Input.GetButtonDown("Jump");
            if (wouldJump)
            {
                Rigidbody body = GetComponent<Rigidbody>();
                Vector3 velocity = body.velocity;
                velocity.y = JumpVelocity;
                body.velocity = velocity;
            }


        }
    }
}
