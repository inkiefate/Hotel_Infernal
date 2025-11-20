using UnityEngine;
using UnityEngine.AI;

// Demonio que se mueve solo cuando el jugador NO lo está mirando
public class DemonioMiradaFP : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;          // Transform del jugador (posición a seguir)
    public Camera camaraJugador;       // Cámara en primera persona usada para detectar la mirada
    public GameOverUITMP interfazGameOver;

    [Header("Movimiento")]
    public float velocidadPersecucion = 2f;  // Velocidad lenta al perseguir
    public float distanciaMatar = 1.2f;      // Distancia de muerte si no se usa trigger

    [Header("Detección de mirada")]
    public float umbralDot = 0.7f;           // Sensibilidad del ángulo de visión (0.7 ≈ 45°)
    public float maxDistanciaVista = 30f;    // Máxima distancia en la que el jugador puede verlo
    public LayerMask mascaraObstaculos;      // Capas que bloquean la visión

    private NavMeshAgent agente;

    void Start()
    {
        // Obtener NavMeshAgent y configurar movimiento inicial
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadPersecucion;
        agente.stoppingDistance = 0f;

        // Asigna cámara principal si no se asignó manualmente
        if (camaraJugador == null && Camera.main != null)
            camaraJugador = Camera.main;
    }

    void Update()
    {
        // Si falta referencia, el script no hace nada
        if (jugador == null || camaraJugador == null) return;

        // Comprobar si el jugador está mirando al demonio
        bool meMira = JugadorMeMiraConLineaDeVista();

        // Si el jugador lo mira, el demonio se queda quieto
        if (meMira)
        {
            agente.isStopped = true;
        }
        else
        {
            // Si no lo mira, persigue al jugador
            agente.isStopped = false;
            agente.SetDestination(jugador.position);
        }

        // Matar por distancia si no se usa trigger
        if (!meMira && Vector3.Distance(transform.position, jugador.position) <= distanciaMatar)
        {
            ActivarGameOver();
        }
    }

    // Detecta si el jugador realmente está mirando al demonio con FOV + línea de visión
    bool JugadorMeMiraConLineaDeVista()
    {
        // Dirección desde la cámara hacia el demonio
        Vector3 dirHaciaDemonio = (transform.position - camaraJugador.transform.position).normalized;

        // Ángulo: comparación mediante Dot Product
        float dot = Vector3.Dot(camaraJugador.transform.forward, dirHaciaDemonio);

        // Si el demonio está fuera del ángulo permitido, no lo está mirando
        if (dot < umbralDot) return false;

        // Verificar distancia máxima de detección
        float distancia = Vector3.Distance(camaraJugador.transform.position, transform.position);
        if (distancia > maxDistanciaVista) return false;

        // Raycast para comprobar si hay objetos bloqueando la vista
        if (Physics.Raycast(
                camaraJugador.transform.position,
                dirHaciaDemonio,
                out RaycastHit hit,
                distancia,
                mascaraObstaculos,
                QueryTriggerInteraction.Ignore))
        {
            // Si lo primero que golpea NO es el demonio, la línea de visión está bloqueada
            if (hit.transform != transform) return false;
        }

        // Si pasa todas las pruebas, el jugador lo está mirando
        return true;
    }

    // Si usa colisionador como detección de muerte
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivarGameOver();
        }
    }

    // Llama a la interfaz para mostrar Game Over
    void ActivarGameOver()
    {
        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
            Time.timeScale = 0f;  // Pausa el juego
        }
        else
        {
            Debug.LogWarning("Interfaz Game Over no asignada.");
        }
    }
}
