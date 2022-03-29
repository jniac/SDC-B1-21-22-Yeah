using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public bool invicible = false;
    public float invicibleUntil = -1;

    public void ApplyDamage(float damage)
    {
        if (invicible || invicibleUntil > Time.time)
            return;

        health = Mathf.Max(0f, health - damage);

        if (health == 0f)
            Destroy(gameObject);
    }

    public void InvicibleUntil(float duration = 0.5f)
    {
        invicibleUntil = Mathf.Max(invicibleUntil, Time.time + duration);
    }
}
