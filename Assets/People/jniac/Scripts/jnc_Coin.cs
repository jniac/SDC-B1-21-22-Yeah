using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_Coin : MonoBehaviour
{
    public enum CoinType
    {
        Normal,
        Purple,
    }

    public CoinType type = CoinType.Normal;

    public GameObject[] particles;

    public int particleCount = 10;

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        Boom.FromPoint(transform.position, particles, particleCount);
    }
}
