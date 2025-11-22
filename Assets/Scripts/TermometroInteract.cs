using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TermometroInteract : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject canvasTermometro;   // Canvas del termómetro
    public Slider sliderTemperatura;      // Slider para ajustar la temperatura
    public TMP_Text textoTemperatura;     // Texto que muestra el valor actual
    public Button botonConfirmar;         // Botón para confirmar

    [Header("Rango correcto")]
    public float minCorrecto = 23f;
    public float maxCorrecto = 25f;

    private bool abierto = false;

    public DemonBehaviour2 demonio2;       // Referencia al demonio
    public PlayerMovement playerMovement;  // Referencia al movimiento del jugador (para saber si lleva algo)



    void Start()
    {
        canvasTermometro.SetActive(false);
        sliderTemperatura.onValueChanged.AddListener(ActualizarTexto);
        botonConfirmar.onClick.AddListener(ValidarTemperatura);
    }

    void Update()
    {
        // Abrir el termómetro con E si miras al objeto
        // Nueva condición: solo si NO lleva objeto
        if (!abierto && !playerMovement.EstaLlevandoObjeto && DetectarTermometro() && Input.GetKeyDown(KeyCode.E))
        {
            abierto = true;
            canvasTermometro.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Canvas del termómetro abierto");
        }
    }


    bool DetectarTermometro()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // SphereCast para detectar objetos cerca del centro de la pantalla
        if (Physics.SphereCast(ray, 0.5f, out hit, 3.5f))
        {
            // Solo devuelve true si el objeto golpeado es este termómetro
            return hit.collider != null && hit.collider.gameObject == gameObject;
        }

        return false;
    }


    void ActualizarTexto(float valor)
    {
        textoTemperatura.text = valor.ToString("F1") + " °C";
    }

    void ValidarTemperatura()
    {
        float valor = sliderTemperatura.value;

        if (valor >= minCorrecto && valor <= maxCorrecto)
        {
            Debug.Log("Temperatura correcta: " + valor);
            CerrarCanvas();
            // Aquí puedes marcar la tarea como completada
        }
        else
        {
            Debug.Log("Temperatura incorrecta: " + valor);
            CerrarCanvas();
            if (demonio2 != null)
                demonio2.ActivarPersecucionRapida();
            // Aquí puedes disparar consecuencia (ej. demonio enfadado)
        }
    }

    void CerrarCanvas()
    {
        canvasTermometro.SetActive(false);
        abierto = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
