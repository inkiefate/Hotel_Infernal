using UnityEngine;
using UnityEngine.SceneManagement;

// Administra las tareas del juego, el temporizador general y el comportamiento del demonio
public class GameTaskManager : MonoBehaviour
{
    public CleanerManager cleanerManager;   // Referencia al sistema de limpieza
    public ToallaPickup toallaPickup;       // Referencia al objeto de la toalla
    public DemonBehaviour demonBehaviour;   // Referencia al demonio

    public GameObject juegoFinalizadoCanvas; // Canvas que aparece cuando termina el juego

    public float tiempoTotal = 210f;        // Tiempo total para completar las tareas
    private float tiempoRestante;           // Tiempo que va decreciendo
    private bool persecucionActivada = false; // Si la persecución suave del demonio comenzó
    private bool modoMatarActivado = false;   // Si el demonio entró en modo matar
    private bool juegoFinalizado = false;     // Si el juego terminó

    void Start()
    {
        // Inicializar tiempo restante
        tiempoRestante = tiempoTotal;

        // Asegurarse de que el canvas de finalización esté oculto al inicio
        if (juegoFinalizadoCanvas != null)
            juegoFinalizadoCanvas.SetActive(false);
    }

    void Update()
    {
        // Si el juego ya terminó, permitir reiniciar presionando U
        if (juegoFinalizado)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                Time.timeScale = 1f; // Reactivar tiempo
                SceneManager.LoadScene("MainMenuControlador"); // Volver al menú
            }
            return;
        }

        // Reducir el tiempo restante
        tiempoRestante -= Time.deltaTime;

        // Verificar si se completaron las tareas
        bool limpiezaOk = cleanerManager != null && cleanerManager.LimpiezaCompletada;
        bool toallaOk = toallaPickup != null && toallaPickup.ToallaEntregada;
        bool tareasCompletas = limpiezaOk && toallaOk;

        // Si el jugador completó todas las tareas
        if (tareasCompletas)
        {
            // Si la persecución ya estaba activa pero no en modo matar, calmar al demonio
            if (persecucionActivada && !modoMatarActivado)
            {
                demonBehaviour.Calmar();
                Debug.Log("Tareas completadas: demonio calmado.");
            }

            // Finalizar el juego
            FinalizarJuego();
        }

        // Si queda menos de 60s y no se activó la persecución, activarla
        if (tiempoRestante <= 60f && !persecucionActivada && !tareasCompletas)
        {
            demonBehaviour.ActivarPersecucionSuave();
            persecucionActivada = true;
        }

        // Si el tiempo llega a cero y no está en modo matar, activarlo
        if (tiempoRestante <= 0f && !modoMatarActivado && !tareasCompletas)
        {
            demonBehaviour.ActivarModoMatar();
            modoMatarActivado = true;
        }
    }

    // Maneja la finalización del juego
    void FinalizarJuego()
    {
        juegoFinalizado = true;
        Time.timeScale = 0f; // Pausar el juego

        if (juegoFinalizadoCanvas != null)
        {
            juegoFinalizadoCanvas.SetActive(true);
            Debug.Log("Felicidades, has sobrevivido la primera noche.");
        }
        else
        {
            Debug.LogWarning("Canvas 'juegoFinalizado' no asignado.");
        }
    }

    // Dibujar la barra de tiempo y texto en pantalla
    void OnGUI()
    {
        if (Time.timeScale == 0f || juegoFinalizado) return;

        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = 34;
        estiloTexto.normal.textColor = Color.white;
        estiloTexto.alignment = TextAnchor.UpperLeft;

        float anchoBarra = 520f;
        float altoBarra = 45f;
        float x = 35f;
        float y = 55f;

        // Dibujar fondo gris de la barra
        GUI.color = Color.gray;
        GUI.DrawTexture(new Rect(x, y, anchoBarra, altoBarra), Texture2D.whiteTexture);

        // Dibujar la porción de tiempo restante en color cyan
        float porcentaje = tiempoRestante / tiempoTotal;
        GUI.color = Color.cyan;
        GUI.DrawTexture(new Rect(x, y, anchoBarra * porcentaje, altoBarra), Texture2D.whiteTexture);

        // Mostrar texto del tiempo restante
        string texto = $"Tiempo restante: {tiempoRestante:F1}s";
        GUI.color = Color.white;
        GUI.Label(new Rect(x, y - 45, anchoBarra, 45), texto, estiloTexto);
    }

}
