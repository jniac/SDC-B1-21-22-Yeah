using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jniac_TriggerFireworks : MonoBehaviour
{
    public GameObject[] sources = new GameObject[0];

    public int particuleCount = 10;
    public float velocity = 10;
    public float lifeDuration = 1.5f;

    void Boom()
    {
        if (sources.Length == 0)
        {
            Debug.LogWarning("Hey, j'ai pas de sources!");
            Debug.Break();
        }

        for (int i = 0; i < particuleCount; i += 1)
        {
            GameObject source = sources[Random.Range(0, sources.Length)];
            GameObject clone = Instantiate(source, transform.position, Quaternion.identity);

            clone.transform.localScale = Vector3.one * Random.Range(0.6f, 1f);

            Vector3 direction = Random.onUnitSphere;
            if (direction.y < 0) direction.y = -direction.y;

            clone.GetComponent<Rigidbody>().velocity = direction * velocity * Random.Range(0.2f, 1f);

            float duration = lifeDuration * Random.Range(0.2f, 1f);
            Destroy(clone, duration);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Boom();
    }
}
