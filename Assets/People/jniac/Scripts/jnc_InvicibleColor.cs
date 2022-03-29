using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_InvicibleColor : MonoBehaviour
{
    public Color normal = Color.yellow;
    public Color invicible = Color.white;

    Health health;
    Material material;

    void Start()
    {
        health = GetComponentInParent<Health>();
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        var color = health.IsInvicible ? invicible : normal;
        material.color = Color.Lerp(material.color, color, 0.5f);
    }
}
