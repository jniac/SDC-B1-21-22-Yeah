using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour {

    public float speed = 5f;

    void Update() {
        // Récupération du composant "voisin" (sibling) : Le rigidbody
        Rigidbody body = GetComponent<Rigidbody>();

        // Récupération des inputs
        float inputH = Input.GetAxis("Horizontal"); 
        float inputV = Input.GetAxis("Vertical");

        // Calcul d'une petite variable (float = nombre) pour "décider" du contrôle
        // que le joueur a sur le cube (si pas d'input : 0, si un ou plusieurs input: 1)
        float controlInfluence = Mathf.Clamp01(Mathf.Abs(inputH) + Mathf.Abs(inputV));

        // D'abord on récupère la vélocité actuelle du cube (par le rigidbody)
        Vector3 velocity = body.velocity;
        // On applique l'effet des inputs
        velocity.x = inputH * speed;
        velocity.z = inputV * speed;

        // On applique la vélocité au cube (rigidbody) si l'influence le permet
        // (interpolation linéaire)
        body.velocity = Vector3.Lerp(body.velocity, velocity, controlInfluence);
    }
}
