using UnityEngine;
using UnityEngine.UI;

public class DogTimerUI : MonoBehaviour
{
    public DogTimer timerScript;
    public Text timerText;

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (timerText.enabled) timerText.enabled = false;
            return;
        }

        if (!timerText.enabled) timerText.enabled = true;

        float timeLeft = timerScript.GetTimeRemaining();
        timerText.text = $"Tiempo restante: {timeLeft:F1}s";
    }



}
