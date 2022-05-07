using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lxx_TimeBonus : MonoBehaviour
{
    public float timeBonus = 5f;

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        lxx_TimeAttackLevelManager Manager = FindObjectOfType<lxx_TimeAttackLevelManager>();
        Manager.remainingTime += timeBonus;
    }
}
