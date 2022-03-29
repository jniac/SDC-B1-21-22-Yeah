using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jnc_UIStartScreen : MonoBehaviour
{
    void Start()
    {
        var normal = transform.FindDeepChild("Normal").GetComponent<TMPro.TextMeshProUGUI>();
        normal.text = normal.text.Replace("$n", jnc_LevelManager.Instance.normals.Length.ToString());

        var purple = transform.FindDeepChild("Purple").GetComponent<TMPro.TextMeshProUGUI>();
        purple.text = purple.text.Replace("$n", jnc_LevelManager.Instance.purples.Length.ToString());
    }
}
