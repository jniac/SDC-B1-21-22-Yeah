using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class jnc_CameraRotator : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera vcam;
    public float offset = 40;

    void Update()
    {
        var target = vcam?.Follow;

        if (target != null)
        {
            var p = transform.InverseTransformPoint(target.transform.position);
            var a = Mathf.Atan2(p.z, p.x) * Mathf.Rad2Deg;
            
            var euler = vcam.transform.rotation.eulerAngles;
            euler.y = -a + offset;
            vcam.transform.rotation = Quaternion.Euler(euler);
        }

        offset += Input.GetAxis("Horizontal2");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(0.1f, 100f, 0.1f));
    }
}
