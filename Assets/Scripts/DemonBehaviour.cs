using UnityEngine;
using UnityEngine.AI;

public class DemonBehaviour : MonoBehaviour
{
    public Transform jugador;                     // Referencia al jugador
    public GameOverUITMP interfazGameOver;        // Referencia a la interfaz de Game Over
    public Transform[] puntosPatrulla;           // Lista de puntos por donde patrulla
    
    public float velocidadNormal = 5.5f;          // Velocidad cuando patrulla o persigue suave
    public float velocidadMatar = 8f;             // Velocidad cuando esta en modo matar
    public float distanciaMinima = 6f;            // Distancia a la que se detiene cuando no mata

    private NavMeshAgent agente;                  // Componente de navegacion
    private bool enfadado = false;                // Persigue sin matar
    private bool faseFinal = false;               // Persigue para matar
    private int puntoActual = -1;                 // Indice del punto de patrulla actual

    void Start()
    {
        // Obtener el NavMeshAgent y configurar valores iniciales
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;

        // Empezar patrullando
        MoverAPuntoDePatrulla();
    }

    void Update()
    {
        // Mientras esta tranquilo y no en modo matar, patrulla
        if (!enfadado && !faseFinal)
        {
            // Cuando llega al punto, selecciona uno nuevo
            if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
            {
                MoverAPuntoDePatrulla();
            }
        }

        // Si esta enfadado o en fase final, persigue al jugador
        if ((enfadado || faseFinal) && jugador != null)
        {
            agente.SetDestination(jugador.position);
        }
    }

    // Selecciona un punto de patrulla al azar y se dirige a el
    void MoverAPuntoDePatrulla()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        int nuevoIndice;

        // Asegurar que no tome el mismo punto consecutivamente
        do
        {
            nuevoIndice = Random.Range(0, puntosPatrulla.Length);
        } 
        while (nuevoIndice == puntoActual && puntosPatrulla.Length > 1);

        puntoActual = nuevoIndice;

        // Moverse hacia el punto seleccionado
        agente.SetDestination(puntosPatrulla[puntoActual].position);
    }

    // Activa persecucion sin matar
    public void ActivarPersecucionSuave()
    {
        enfadado = true;
        faseFinal = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
    }

    // Activa modo matar, teletransporta cerca del jugador y acelera
    public void ActivarModoMatar()
    {
        enfadado = true;
        faseFinal = true;
        agente.speed = velocidadMatar;
        agente.stoppingDistance = 0f;
        Teletransportarse();
    }

    // Intenta calmar al demonio, solo si no esta en modo matar
    public void Calmar()
    {
        // No puede ser calmado si ya esta en fase final
        if (faseFinal) return;

        // Vuelve a patrullar
        enfadado = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
        MoverAPuntoDePatrulla();
    }

    // Teletransporta al demonio cerca del jugador usando NavMesh
    public void Teletransportarse()
    {
        if (jugador != null)
        {
            // Calcular un punto aleatorio alrededor del jugador
            Vector3 direccionAleatoria = Random.onUnitSphere;
            direccionAleatoria.y = 0;
            direccionAleatoria.Normalize();
            
            Vector3 destino = jugador.position + direccionAleatoria * 12f;

            NavMeshHit hit;

            // Buscar una posicion valida en el NavMesh
            if (NavMesh.SamplePosition(destino, out hit, 10f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agente.SetDestination(jugador.position);
            }
        }
    }

    // Detecta si atrapo al jugador durante la fase final
    void OnTriggerEnter(Collider other)
    {
        if (faseFinal && other.CompareTag("Player"))
        {
            if (interfazGameOver != null)
            {
                // Mostrar interfaz de Game Over y pausar el juego
                interfazGameOver.ShowGameOverMessage();
                Time.timeScale = 0f;
            }
        }
    }
}