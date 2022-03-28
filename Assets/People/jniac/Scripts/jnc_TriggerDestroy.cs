using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_TriggerDestroy : MonoBehaviour
{
    void OnTriggerStay(Collider other) 
    {
        // Si invicible, alors invicible (return).
        if (other.attachedRigidbody?.gameObject.tag == "Player" && PlayModeManager.Test(PlayMode.NeverDie))
            return;

        Destroy(other.attachedRigidbody.gameObject);
    }
}
