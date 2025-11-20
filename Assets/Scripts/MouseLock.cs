using UnityEngine;

// Controla la rotación de la cámara y el cuerpo del jugador usando el mouse
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;   // Sensibilidad del mouse
    public Transform playerBody;            // Transform del cuerpo del jugador (rotación horizontal)
    public Transform cameraTransform;       // Transform de la cámara (rotación vertical)

    private float xRotation = 0f;           // Rotación acumulada en el eje X (vertical)

    void Start()
    {
        // Asigna el cuerpo del jugador al objeto actual si no se asigna manualmente
        playerBody = transform;

        // Cargar sensibilidad y volumen guardados desde SettingsManager
        if (SettingsManager.Instance != null)
        {
            mouseSensitivity = SettingsManager.Instance.sensibilidad;
            AudioListener.volume = SettingsManager.Instance.volumen;
        }
    }

    void Update()
    {
        // Obtener el movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación vertical de la cámara
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar rotación vertical
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal del cuerpo del jugador
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // Permite actualizar la sensibilidad desde otros scripts (por ejemplo, menú de opciones)
    public void ActualizarSensibilidad(float nuevaSensibilidad)
    {
        mouseSensitivity = nuevaSensibilidad;
        Debug.Log("Sensibilidad actualizada en MouseLook: " + mouseSensitivity);
    }
}
