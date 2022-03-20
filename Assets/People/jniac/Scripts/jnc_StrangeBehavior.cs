using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_StrangeBehavior : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Block"))
        {
            transform.Find("Ghost").gameObject.SetActive(false);
            transform.Find("GreyCube").gameObject.SetActive(true);
        }

        if (Input.GetButtonUp("Block"))
        {
            transform.Find("Ghost").gameObject.SetActive(true);
            transform.Find("GreyCube").gameObject.SetActive(false);
        }
    }
}
