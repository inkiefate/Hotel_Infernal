using UnityEngine;
using System.Collections.Generic;

// Administrador que controla la limpieza de varios cubos "limpiables"
public class CleanerManager : MonoBehaviour
{
    // Cámara del jugador para lanzar el raycast
    public Camera playerCamera;

    // Lista de objetos que pueden ser limpiados
    public GameObject[] cubosLimpiables;

    // Referencia al script de movimiento del jugador (para saber si lleva un objeto)
    public PlayerMovement playerMovement;

    // Diccionario para llevar el conteo de interacciones por cubo
    private Dictionary<GameObject, int> interacciones = new Dictionary<GameObject, int>();

    // Cubo que el jugador está mirando actualmente
    private GameObject cuboActual;

    // Indica si el jugador está cerca del cubo y mirando hacia él
    private bool cerca = false;

    // Indica si todas las tareas de limpieza han terminado
    private bool limpiezaCompletada = false;

    void Start()
    {
        // Inicializa cada cubo activándolo y asegurándose de que tenga el comportamiento CleanerBehavior
        foreach (GameObject cubo in cubosLimpiables)
        {
            // Activa el cubo por si estaba apagado
            cubo.SetActive(true);

            // Inicia su contador de interacciones en cero
            interacciones[cubo] = 0;

            // Si el cubo no tiene el script CleanerBehavior, se le añade uno
            if (!cubo.TryGetComponent(out CleanerBehavior behavior))
                cubo.AddComponent<CleanerBehavior>();
        }
    }

    void Update()
    {
        // Si ya se completó la limpieza, no ejecutar más lógica
        if (limpiezaCompletada) return;

        // Bloqueo: si el jugador lleva un objeto (por ejemplo una toalla), no puede limpiar
        if (playerMovement != null && playerMovement.EstaLlevandoObjeto)
        {
            cerca = false;
            cuboActual = null;
            return;
        }

        // Raycast hacia adelante desde la cámara del jugador, con un alcance de 2 metros
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            // Si lo que mira el jugador tiene la etiqueta "Cleanable", se considera un cubo a limpiar
            if (hit.collider.CompareTag("Cleanable"))
            {
                cuboActual = hit.collider.gameObject;
                cerca = true;
            }
            else
            {
                cerca = false;
            }
        }
        else
        {
            cerca = false;
        }

        // Si el jugador está mirando un cubo y presiona E
        if (cerca && Input.GetKeyDown(KeyCode.E) && cuboActual != null)
        {
            // Verifica que el cubo esté en el diccionario de interacciones
            if (interacciones.ContainsKey(cuboActual))
            {
                // Suma una interacción
                interacciones[cuboActual]++;
                Debug.Log("Cubo " + cuboActual.name + ": " + interacciones[cuboActual] + " interacciones.");

                // Si llegó a 3 interacciones, se "limpia" el cubo desactivándolo
                if (interacciones[cuboActual] >= 3)
                {
                    cuboActual.SetActive(false);
                    interacciones[cuboActual] = 0;
                }

                // Si todos los cubos están desactivados, la limpieza termina
                if (TodosCubosDesactivados())
                {
                    limpiezaCompletada = true;
                    Debug.Log("Tarea de limpieza completada.");
                }
            }
        }
    }

    // Revisa si cada cubo limpiable está desactivado
    bool TodosCubosDesactivados()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            if (cubo.activeSelf)
                return false;
        }
        return true;
    }

    // Propiedad pública para consultar desde otros scripts si se completó la limpieza
    public bool LimpiezaCompletada => limpiezaCompletada;
}
