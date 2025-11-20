using UnityEngine;
using UnityEngine.UI;

// Administra las opciones del juego como sensibilidad del mouse y volumen
public class OptionsManager : MonoBehaviour
{
    public Slider sensibilidadSlider;  // Slider para ajustar la sensibilidad del mouse
    public Slider volumenSlider;       // Slider para ajustar el volumen del juego

    public MouseLook mouseLookScript;  // Referencia al script MouseLook para actualizar sensibilidad

    void Start()
    {
        // Inicializar sliders con los valores guardados en SettingsManager
        sensibilidadSlider.value = SettingsManager.Instance.sensibilidad;
        volumenSlider.value = SettingsManager.Instance.volumen;

        // Suscribir los sliders a sus funciones correspondientes
        sensibilidadSlider.onValueChanged.AddListener(SetSensibilidad);
        volumenSlider.onValueChanged.AddListener(SetVolumen);
    }

    // Cambia la sensibilidad y actualiza el script MouseLook
    public void SetSensibilidad(float value)
    {
        SettingsManager.Instance.SetSensibilidad(value);

        if (mouseLookScript != null)
            mouseLookScript.ActualizarSensibilidad(value);
    }

    // Cambia el volumen usando SettingsManager
    public void SetVolumen(float value)
    {
        SettingsManager.Instance.SetVolumen(value);
    }
}
