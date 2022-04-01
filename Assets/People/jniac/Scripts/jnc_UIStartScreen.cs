using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE:
// Update est overkill ici, mais de meilleures solutions impliquent trop de 
// complexités (SendMessage, Events) pour des débutants. 
// Donc pour l'instant osef. 

public class jnc_UIStartScreen : MonoBehaviour
{
    TMPro.TextMeshProUGUI normalText, purpleText;

    void Start()
    {
        normalText = transform
            .DeepFind("Normal")
            .GetComponent<TMPro.TextMeshProUGUI>();

        purpleText = transform
            .DeepFind("Purple")
            .GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        var (normalTotal, purpleTotal) = jnc_LevelManager.Instance.GetCoinTotal();

        normalText.text = $"Normal ({normalTotal})";
        purpleText.text = $"Purple ({purpleTotal})";
    }
}
