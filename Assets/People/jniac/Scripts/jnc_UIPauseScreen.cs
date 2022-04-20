using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

    // NOTE:
    // Update est overkill ici, mais de meilleures solutions impliquent trop de 
    // complexités (SendMessage, Events) pour des débutants. 
    // Donc pour l'instant osef. 

public class jnc_UIPauseScreen : MonoBehaviour
{
    TMPro.TextMeshProUGUI timerText, normalText, purpleText;

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
        var (normalCount, purpleCount) = jnc_LevelManager.Instance.GetCoinCount();
        var (normalTotal, purpleTotal) = jnc_LevelManager.Instance.GetCoinTotal();

        normalText.text = $"Normal ({normalCount}/{normalTotal})";
        purpleText.text = $"Purple ({purpleCount}/{purpleTotal})";
    }
}
