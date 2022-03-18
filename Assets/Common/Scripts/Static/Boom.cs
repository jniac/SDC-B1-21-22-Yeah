using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Boom
{
    public static bool IsQuitting { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    static void StaticStart()
    {
        Application.quitting += () => IsQuitting = true;
    }

    public static void FromPoint(
        Vector3 point,
        int particlePoint,
        IList<GameObject> sources,
        float lifeDuration = 1.5f,
        float velocity = 10f
    )
    {
        if (IsQuitting)
            return;

        int sourcesCount = sources.Count();

        if (sourcesCount == 0)
            return;

        for (int i = 0; i < particlePoint; i += 1)
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
}
