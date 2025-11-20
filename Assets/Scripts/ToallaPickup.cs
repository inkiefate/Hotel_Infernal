using UnityEngine;

// Permite al jugador recoger y entregar la toalla en el juego
public class ToallaPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;     // Referencia al script de movimiento del jugador
    public Transform toallaAnchor;            // Punto donde se colocará la toalla al recogerla
    public GameObject toallaVisualPrefab;     // Prefab que representa la toalla en el baño
    public Transform puntoColocacion;         // Punto en el baño donde se colocará la toalla

    public Camera camaraJugador;              // Cámara del jugador para detectar interacción
    public float radioInteraccion = 0.5f;     // Radio del SphereCast para detectar la toalla
    public float distanciaInteraccion = 3.5f; // Distancia máxima para interactuar

    private bool recogida = false;            // Indica si la toalla ha sido recogida
    private bool entregada = false;           // Indica si la toalla ha sido entregada

    void Update()
    {
        // Detectar si el jugador está mirando la toalla y presiona E para recogerla
        if (!recogida && DetectarToalla() && Input.GetKeyDown(KeyCode.E))
        {
            recogida = true;
            playerMovement.LlevarObjeto(true); // Indica al jugador que lleva un objeto

            // Ajustar la posición, rotación y escala de la toalla en la mano
            transform.SetParent(toallaAnchor);
            transform.localPosition = new Vector3(0, -0.4f, 0);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.5f;

            // Desactivar el collider para evitar colisiones mientras se lleva
            GetComponent<Collider>().enabled = false;

            Debug.Log("Toalla recogida");
        }

        // Detectar si el jugador está cerca de la zona de entrega y presiona E
        if (recogida && !entregada)
        {
            Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("EntregaToalla"))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        entregada = true;
                        playerMovement.LlevarObjeto(false); // El jugador deja de llevar el objeto
                        gameObject.SetActive(false);        // Desactivar la toalla en el mundo

                        // Instanciar la toalla visual en el baño
                        if (toallaVisualPrefab != null && puntoColocacion != null)
                        {
                            Vector3 offset = new Vector3(0, 0.5f, 0);
                            Instantiate(toallaVisualPrefab, puntoColocacion.position + offset, puntoColocacion.rotation);
                        }

                        Debug.Log("Toalla entregada y colocada en el baño");
                    }
                }
            }
        }
    }

    // Detecta si el jugador está mirando la toalla usando un SphereCast
    bool DetectarToalla()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
        {
            return hit.collider != null && hit.collider.gameObject == gameObject;
        }

        return false;
    }

    // Propiedad pública para consultar si la toalla ha sido entregada
    public bool ToallaEntregada => entregada;
}
