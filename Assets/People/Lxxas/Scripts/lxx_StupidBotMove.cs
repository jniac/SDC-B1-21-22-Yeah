using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lxx_StupidBotMove : MonoBehaviour
{
    public float velocity = 2f;
    public Transform target;

    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;

        direction = direction.normalized * velocity;
        body.velocity = direction;
    }
}
