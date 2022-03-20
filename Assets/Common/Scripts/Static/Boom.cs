using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Boom
{
    public static bool IsQuitting { get; private set; }

    static bool initialized = false;
    [RuntimeInitializeOnLoadMethod]
    static void StaticStart()
    {
        if (initialized == false)
        {
            // https://docs.unity3d.com/Manual/DomainReloading.html
            initialized = true;
            Application.quitting += () => IsQuitting = true;
        }
    }

    public static void FromPoint(
        Vector3 point,
        int particleCount,
        IList<GameObject> sources,
        float lifeDuration = 1.5f,
        float velocity = 10f
    )
    {
        StaticStart();

        if (IsQuitting)
            return;

        int sourcesCount = sources.Count();

        if (sourcesCount == 0)
            return;

        for (int i = 0; i < particleCount; i += 1)
        {
            GameObject source = sources[Random.Range(0, sourcesCount)];
            GameObject clone = GameObject.Instantiate(source, point, Quaternion.identity);

            clone.transform.localScale = Vector3.one * Random.Range(0.6f, 1f);

            Vector3 direction = Random.onUnitSphere;
            if (direction.y < 0) direction.y = -direction.y;

            clone.GetComponent<Rigidbody>().velocity = direction * velocity * Random.Range(0.2f, 1f);

            float duration = lifeDuration * Random.Range(0.2f, 1f);
            GameObject.Destroy(clone, duration);
        }
    }

    public static void Trail(
        Vector3 origin,
        Vector3 destination,
        int particleCount,
        IList<GameObject> sources,
        float lifeDuration = 0.4f,
        float trailVelocity = 30f,
        float normalVelocity = 2f,
        float radius = 0.5f
    )
    {
        StaticStart();

        if (IsQuitting)
            return;

        int sourcesCount = sources.Count();

        if (sourcesCount == 0)
            return;

        Vector3 delta = destination - origin;
        Vector3 direction = delta.normalized;

        for (int i = 0; i < particleCount; i += 1)
        {
            Vector3 point = origin + delta * Random.Range(0f, 1f);
            GameObject source = sources[Random.Range(0, sourcesCount)];
            GameObject clone = GameObject.Instantiate(source, point, Quaternion.identity);

            clone.transform.localScale = Vector3.one * Random.Range(0.6f, 1f);

            clone.GetComponent<Rigidbody>().velocity = direction * trailVelocity * Random.Range(0.2f, 1f);

            float duration = lifeDuration * Random.Range(0.2f, 1f);
            GameObject.Destroy(clone, duration);
        }
    }
}
