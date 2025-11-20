using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// Comportamiento alternativo del demonio con persecución temporal y rotación hacia el jugador
public class DemonBehaviour2 : MonoBehaviour
{
    // Referencia al jugador
    public Transform jugador;

    // Velocidad a la que el demonio rota para mirar al jugador
    public float velocidadRotacion = 3f;

    // Interfaz de Game Over
    public GameOverUITMP interfazGameOver;

    // NavMeshAgent para mover al demonio
    private NavMeshAgent agente;

    // Indica si el demonio está en modo persecución
    private bool enfadado = false;

    // Punto inicial al que regresará al calmarse
    private Vector3 puntoOrigen;

    void Start()
    {
        // Obtiene el NavMeshAgent y configura valores iniciales
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            agente.isStopped = true;        // No se mueve al inicio
            agente.stoppingDistance = 0f;   // Puede acercarse completamente al jugador
            agente.autoBraking = false;     // Evita desaceleración automática
            agente.updateRotation = true;   // Permite al agente rotar automáticamente
        }

        // Guarda la posición inicial para volver después
        puntoOrigen = transform.position;
    }

    void Update()
    {
        if (jugador != null)
        {
            // Calcula dirección hacia el jugador sin inclinar el demonio verticalmente
            Vector3 direccion = jugador.position - transform.position;
            direccion.y = 0;

            // Rota suavemente hacia el jugador
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
            }

            // Si el demonio está enfadado, activa movimiento hacia el jugador
            if (enfadado && agente != null)
            {
                agente.isStopped = false;
                agente.SetDestination(jugador.position);
            }
        }
    }

    // Activa persecución muy rápida durante un tiempo limitado
    public void ActivarPersecucionRapida()
    {
        if (agente != null && jugador != null)
        {
            agente.speed = 25f;   // Velocidad muy alta
            enfadado = true;

            // Después de 10 segundos se calma automáticamente
            StartCoroutine(CalmarDespuesDeTiempo(10f));
        }
    }

    // Corrutina que calma al demonio después de cierto tiempo
    private IEnumerator CalmarDespuesDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);

        enfadado = false;

        if (agente != null)
        {
            agente.speed = 5f;         // Velocidad normal
            agente.isStopped = false;  // Puede moverse
            agente.SetDestination(puntoOrigen); // Regresa a su punto inicial
        }
    }

    // Detecta si el demonio enfadado golpea al jugador
    void OnTriggerEnter(Collider other)
    {
        if (enfadado && other.CompareTag("Player"))
        {
            if (interfazGameOver != null)
            {
                interfazGameOver.ShowGameOverMessage();
                Time.timeScale = 0f; // Pausa el juego
            }
        }
    }
}
