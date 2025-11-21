using UnityEngine;
using TMPro;          // Para TMP_InputField
using UnityEngine.UI; // Para el botón

public class TelefonoInteract : MonoBehaviour
{
    public Camera camaraJugador;           // Cámara del jugador para detectar interacción
    public float radioInteraccion = 0.5f;  // Radio del SphereCast
    public float distanciaInteraccion = 3.5f;

    public GameObject canvasTelefono;      // Canvas que contiene el InputField y botón
    public TMP_InputField inputCodigo;     // Campo de texto TMP para introducir el código
    public Button botonConfirmar;          // Botón para confirmar el código

    public DemonBehaviour2 demonio2;       // Referencia al demonio
    public PlayerMovement playerMovement;  // Referencia al movimiento del jugador (para saber si lleva algo)

    private bool abierto = false;          // Si el canvas está abierto

    void Start()
    {
        // Aseguramos que el canvas esté oculto al inicio
        canvasTelefono.SetActive(false);

        // Suscribimos el botón al método de validación
        botonConfirmar.onClick.AddListener(ValidarCodigo);
    }

    void Update()
    {
        // Detectar si miramos al teléfono y pulsamos E
        // Nueva condición: solo si NO lleva objeto
        if (!abierto && !playerMovement.EstaLlevandoObjeto && DetectarTelefono() && Input.GetKeyDown(KeyCode.E))
        {
            abierto = true;
            canvasTelefono.SetActive(true);   // Mostrar el canvas
            inputCodigo.text = "";            // Limpiar campo

            // Desbloquear el cursor para poder escribir y hacer clic
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Canvas del teléfono abierto");
        }
    }

    bool DetectarTelefono()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;

        return false;
    }

    // Método público para que aparezca en el OnClick del botón
    public void ValidarCodigo()
    {
        string codigoIngresado = inputCodigo.text;

        if (codigoIngresado == "HAB-02")
        {
            Debug.Log("Código correcto, todo sigue igual");
            CerrarCanvas();
        }
        else
        {
            Debug.Log("Código incorrecto, demonio enfadado!");
            CerrarCanvas();

            if (demonio2 != null)
                demonio2.ActivarPersecucionRapida();
        }
    }

    // Método auxiliar para cerrar el canvas y volver a bloquear el cursor
    void CerrarCanvas()
    {
        canvasTelefono.SetActive(false);
        abierto = false;

        // Volver a bloquear el cursor para seguir jugando
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

