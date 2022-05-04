using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE:
// Update est overkill ici, mais de meilleures solutions impliquent trop de 
// complexités (SendMessage, Events) pour des débutants. 
// Donc pour l'instant osef. 

public class jnc_UIWinScreen : MonoBehaviour
{
    TMPro.TextMeshProUGUI timerText;

    void Start()
    {
        timerText = transform
            .DeepFind("Timer")
            .GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        var (min, sec, ms) = jnc_LevelManager.Instance.GetSessionTimeStrings();
        timerText.text = $"{min}:{sec}:{ms}";
    }
}
