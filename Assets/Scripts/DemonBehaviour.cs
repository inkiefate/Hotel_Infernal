using UnityEngine;
using UnityEngine.AI;

// Comportamiento del demonio que patrulla, persigue y mata al jugador
public class DemonBehaviour : MonoBehaviour
{
    // Referencia al jugador
    public Transform jugador;

    // Referencia a la interfaz de Game Over
    public GameOverUITMP interfazGameOver;

    // Lista de puntos por donde el demonio patrullará
    public Transform[] puntosPatrulla;

    // Velocidades según el estado del demonio
    public float velocidadNormal = 5.5f;
    public float velocidadMatar = 8f;

    // Distancia a la que debería detenerse cuando no está en modo matar
    public float distanciaMinima = 6f;

    // Componentes y estados internos
    private NavMeshAgent agente;
    private bool enfadado = false;   // Persigue sin matar
    private bool faseFinal = false;  // Persigue para matar
    private int puntoActual = -1;    // Índice del punto de patrulla actual

    void Start()
    {
        // Obtiene el NavMeshAgent y configura valores iniciales
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;

        // Empieza patrullando
        MoverAPuntoDePatrulla();
    }

    void Update()
    {
        // Mientras está tranquilo y no en modo matar, patrulla
        if (!enfadado && !faseFinal)
        {
            // Cuando llega al punto, selecciona uno nuevo
            if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
            {
                MoverAPuntoDePatrulla();
            }
        }

        // Si está enfadado o en fase final, persigue al jugador
        if ((enfadado || faseFinal) && jugador != null)
        {
            // Solo actualiza el destino si está lejos del jugador
            if (Vector3.Distance(agente.destination, jugador.position) > 1f)
            {
                agente.SetDestination(jugador.position);
            }
        }
    }

    // Selecciona un punto de patrulla al azar y se dirige a él
    void MoverAPuntoDePatrulla()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        int nuevoIndice;

        // Asegura que no tome el mismo punto consecutivamente
        do
        {
            nuevoIndice = Random.Range(0, puntosPatrulla.Length);
        } 
        while (nuevoIndice == puntoActual && puntosPatrulla.Length > 1);

        puntoActual = nuevoIndice;

        // Se mueve hacia el punto seleccionado
        agente.SetDestination(puntosPatrulla[puntoActual].position);
        Debug.Log("Demonio patrullando hacia: " + puntosPatrulla[puntoActual].name);
    }

    // Activa persecución sin matar
    public void ActivarPersecucionSuave()
    {
        enfadado = true;
        faseFinal = false;

        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;

        Debug.Log("Demonio en modo persecución suave.");
    }

    // Activa modo matar, teletransporta cerca del jugador y acelera
    public void ActivarModoMatar()
    {
        faseFinal = true;

        agente.speed = velocidadMatar;
        agente.stoppingDistance = 0f;

        Teletransportarse();

        Debug.Log("Demonio en modo matar.");
    }

    // Intenta calmar al demonio, solo si no está en modo matar
    public void Calmar()
    {
        // No puede ser calmado si ya está en fase final
        if (faseFinal)
        {
            Debug.Log("No se puede calmar al demonio en modo matar.");
            return;
        }

        // Si ya estaba calmado, no hace nada
        if (!enfadado)
        {
            Debug.Log("El demonio ya está calmado.");
            return;
        }

        // Vuelve a patrullar
        enfadado = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;

        MoverAPuntoDePatrulla();

        Debug.Log("Demonio calmado: vuelve a patrullar.");
    }

    // Teletransporta al demonio cerca del jugador usando NavMesh
    public void Teletransportarse()
    {
        if (jugador != null)
        {
            // Calcula un punto frente al jugador
            Vector3 destino = jugador.position + jugador.forward * 1.5f;

            NavMeshHit hit;

            // Busca una posición válida en el NavMesh alrededor del punto calculado
            if (NavMesh.SamplePosition(destino, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                transform.LookAt(jugador.position);

                // Inmediatamente comienza a perseguirlo
                agente.SetDestination(jugador.position);
            }
        }
    }

    // Detecta si atrapó al jugador durante la fase final
    void OnTriggerEnter(Collider other)
    {
        if (faseFinal && other.CompareTag("Player"))
        {
            Debug.Log("Trigger detectado con el jugador.");

            if (interfazGameOver != null)
            {
                // Muestra interfaz de Game Over y pausa el juego
                interfazGameOver.ShowGameOverMessage();
                Time.timeScale = 0f;

                Debug.Log("El demonio ha atrapado al jugador.");
            }
            else
            {
                Debug.LogWarning("Interfaz Game Over no asignada.");
            }
        }
    }
}






