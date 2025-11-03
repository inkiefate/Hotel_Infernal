using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Slider sensibilidadSlider;
    public Slider volumenSlider;

    public MouseLook mouseLookScript;

    void Start()
    {
        sensibilidadSlider.value = 120;
        volumenSlider.value = AudioListener.volume;

        sensibilidadSlider.onValueChanged.AddListener(SetSensibilidad);
        volumenSlider.onValueChanged.AddListener(SetVolumen);
    }

    public void SetSensibilidad(float value)
    {
        mouseLookScript.mouseSensitivity = value;
    }

    public void SetVolumen(float value)
    {
        AudioListener.volume = value;
    }
}
