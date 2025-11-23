using UnityEngine;

public class PatitoPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;     // Movimiento del jugador
    public Transform patitoAnchor;            // Punto donde se sujeta el patito
    public GameObject patitoVisualPrefab;     // Prefab visual que aparece en la bañera
    public Transform puntoColocacion;         // Lugar exacto de colocación
    public Camera camaraJugador;              // Cámara del jugador
    public float radioInteraccion = 0.5f;     // Radio del SphereCast
    public float distanciaInteraccion = 3.5f; // Distancia máxima

    private bool recogido = false;            // Si el patito ya fue recogido
    private bool entregado = false;           // Si ya se entregó
    private bool cerca = false;               // Si estamos mirando el patito
    private bool cercaBanera = false;         // Si estamos cerca de la bañera

    void Update()
    {
        // Detectar patito para recoger
        cerca = DetectarPatito();

        if (!recogido && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
        {
            recogido = true;
            playerMovement.LlevarObjeto(true, false);

            transform.SetParent(patitoAnchor);
            transform.localPosition = new Vector3(0, -0.4f, 0);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.5f;

            GetComponent<Collider>().enabled = false;
            Debug.Log("Patito recogido");
        }

        // Detectar bañera para entregar
        if (recogido && !entregado)
        {
            cercaBanera = DetectarBanera();

            if (cercaBanera && Input.GetKeyDown(KeyCode.E))
            {
                entregado = true;
                playerMovement.SoltarObjeto();
                gameObject.SetActive(false);

                if (patitoVisualPrefab != null && puntoColocacion != null)
                {
                    Vector3 offset = new Vector3(0, 0.5f, 0);
                    Instantiate(patitoVisualPrefab, puntoColocacion.position + offset, puntoColocacion.rotation);
                }

                Debug.Log("Patito entregado");
            }
        }
    }

    // Detectar patito
    bool DetectarPatito()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Detectar bañera con tag
    bool DetectarBanera()
    {
        Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Bañera") || hit.CompareTag("Bañera"))
                return true;
        }
        return false;
    }

    public bool PatitoEntregado => entregado;

    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 40;
        estilo.normal.textColor = Color.white;
        estilo.alignment = TextAnchor.MiddleCenter;
        Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);

        // Mensaje al recoger
        if (cerca && !recogido)
            GUI.Label(mensaje, "Pulsa E para recoger patito", estilo);

        // Mensaje al entregar
        if (recogido && !entregado && cercaBanera)
            GUI.Label(mensaje, "Pulsa E para entregar patito", estilo);
    }
}

