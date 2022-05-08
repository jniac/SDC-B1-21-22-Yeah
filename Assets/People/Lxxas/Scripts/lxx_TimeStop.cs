using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lxx_TimeStop : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        var timeAttack = FindObjectOfType<lxx_TimeAttackLevelManager>();
        timeAttack.timePaused = true;
    }

}
