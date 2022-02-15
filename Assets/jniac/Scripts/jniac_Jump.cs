using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jniac_Jump : MonoBehaviour {

    public float jumpSpeed = 10f;

    bool CanJump() {
        if (Input.GetButtonDown("Jump")) {
            return  GetComponent<GroundDetection>().onGround;
        }
        else {
            return false;
        }
    }

    void Update() {
        
        bool canJump = CanJump();

        if (canJump) {
            
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

