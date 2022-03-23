using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_TriggerDestroy : MonoBehaviour
{
    void OnTriggerEnter(Collider other) 
    {
        Destroy(other.attachedRigidbody.gameObject);
    }
}
