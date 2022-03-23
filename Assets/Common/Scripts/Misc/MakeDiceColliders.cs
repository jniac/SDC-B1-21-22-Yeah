using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MakeDiceColliders : MonoBehaviour
{
    public Vector3 size = Vector3.one;
    public float sizeScalar = 1f;
    public float cornerRadius = 0.1f;
    public bool hideColliders = true;

    bool CollidersAreOk(Collider[] colliders)
    {
        return (
            colliders.Length == 11
            && colliders[0] is BoxCollider
            && colliders[1] is BoxCollider
            && colliders[2] is BoxCollider
            && colliders[3] is SphereCollider
            && colliders[4] is SphereCollider
            && colliders[5] is SphereCollider
            && colliders[6] is SphereCollider
            && colliders[7] is SphereCollider
            && colliders[8] is SphereCollider
            && colliders[9] is SphereCollider
            && colliders[10] is SphereCollider
        );
    }

    void Setup(bool forceDestroy = false)
    {
        var colliders = GetComponents<Collider>();

        if (CollidersAreOk(colliders) == false || forceDestroy)
        {
            foreach (var collider in colliders)
                DestroyImmediate(collider);

            colliders = new Collider[] {
                gameObject.AddComponent<BoxCollider>(),
                gameObject.AddComponent<BoxCollider>(),
                gameObject.AddComponent<BoxCollider>(),
                gameObject.AddComponent<SphereCollider>(),
                gameObject.AddComponent<SphereCollider>(),
                gameObject.AddComponent<SphereCollider>(),
                gameObject.AddComponent<SphereCollider>(),
                gameObject.AddComponent<SphereCollider>(),
                gameObject.AddComponent<SphereCollider>(),
                gameObject.AddComponent<SphereCollider>(),
                gameObject.AddComponent<SphereCollider>(),
            };
        }

        foreach (var collider in colliders)
            collider.hideFlags = hideColliders ? HideFlags.HideInInspector : HideFlags.None;

        var outer = size * sizeScalar;
        var inner = outer - Vector3.one * 2f * cornerRadius;
        var box1 = colliders[0] as BoxCollider;
        var box2 = colliders[1] as BoxCollider;
        var box3 = colliders[2] as BoxCollider;
        box1.size = new Vector3(outer.x, inner.y, inner.z);
        box2.size = new Vector3(inner.x, outer.y, inner.z);
        box3.size = new Vector3(inner.x, inner.y, outer.z);

        var sphere1 = colliders[3] as SphereCollider;
        var sphere2 = colliders[4] as SphereCollider;
        var sphere3 = colliders[5] as SphereCollider;
        var sphere4 = colliders[6] as SphereCollider;
        var sphere5 = colliders[7] as SphereCollider;
        var sphere6 = colliders[8] as SphereCollider;
        var sphere7 = colliders[9] as SphereCollider;
        var sphere8 = colliders[10] as SphereCollider;
        sphere1.radius = cornerRadius;
        sphere2.radius = cornerRadius;
        sphere3.radius = cornerRadius;
        sphere4.radius = cornerRadius;
        sphere6.radius = cornerRadius;
        sphere7.radius = cornerRadius;
        sphere8.radius = cornerRadius;
        sphere5.radius = cornerRadius;
        sphere1.center = new Vector3(+inner.x,  inner.y,  inner.z) / 2f;
        sphere2.center = new Vector3(-inner.x,  inner.y,  inner.z) / 2f;
        sphere3.center = new Vector3(-inner.x, -inner.y,  inner.z) / 2f;
        sphere4.center = new Vector3( inner.x, -inner.y,  inner.z) / 2f;
        sphere5.center = new Vector3( inner.x,  inner.y, -inner.z) / 2f;
        sphere6.center = new Vector3(-inner.x,  inner.y, -inner.z) / 2f;
        sphere7.center = new Vector3(-inner.x, -inner.y, -inner.z) / 2f;
        sphere8.center = new Vector3( inner.x, -inner.y, -inner.z) / 2f;
    }

    PhysicMaterial currentPhysicMaterial;
    public void SetPhysicMaterial(PhysicMaterial physicMaterial)
    {
        if (currentPhysicMaterial != physicMaterial) {
            currentPhysicMaterial = physicMaterial;
            foreach (var collider in GetComponents<Collider>())
                collider.material = physicMaterial;
        }
    }

    void Update()
    {
        if (Application.isPlaying == false)
            Setup();
    }

}
