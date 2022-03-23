using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hiryrune_Jump : MonoBehaviour
{
    public float jumpSpeed = 10f;


    void Update()
    {

        bool canJump = true;

        if (canJump)
        {

            bool wouldJump = Input.GetButtonDown("Jump");

            if (wouldJump)
            {

                // Récupération de la vitesse actuelle (d'abord le rigidbody, ensuite "velocity")
                Rigidbody body = GetComponent<Rigidbody>();
                Vector3 velocity = body.velocity;

                // Modification de la vitesse "verticale" (le saut, "y").
                velocity.y = jumpSpeed;

                // Affectation de la nouvelle vitesse au rigidbody.
                body.velocity = velocity;

            }
        }
    }

}
