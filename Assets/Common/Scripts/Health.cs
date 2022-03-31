using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public float healthMax = 100f;

    public bool invicible = false;
    public float invicibleUntil = -1;

    public float HealthRatio => health / healthMax;
    public bool IsInvicible => invicible || invicibleUntil > Time.time; 

    public void ApplyDamage(float damage)
    {
        if (IsInvicible)
            return;

        health = Mathf.Max(0f, health - damage);

        if (health == 0f)
            Destroy(gameObject);
    }

    public void InvicibleUntil(float duration = 0.5f)
    {
        invicibleUntil = Mathf.Max(invicibleUntil, Time.time + duration);
    }

    void Start()
    {
        InvicibleUntil(0.5f);
    }
}
