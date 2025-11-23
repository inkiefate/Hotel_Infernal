using UnityEngine;

public class BedTaskManager : MonoBehaviour
{
    public Camera playerCamera;
    public BedObjectBehavior[] objetosCama;
    private BedObjectBehavior objetoActual;
    private bool cerca = false;
    private bool tareaCompletada = false;
    private float tiempoMantener = 3f;
    private float contadorMantener = 0f;
    private bool manteniendo = false;
    public PlayerMovement playerMovement;

    // Propiedad pública para que GameTaskManager pueda acceder
    public bool TareaCompletada => tareaCompletada;

    void Update()
    {
        if (tareaCompletada) return;

        if (playerMovement != null && playerMovement.EstaLlevandoObjeto)
        {
            cerca = false;
            objetoActual = null;
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 4f))
        {
            BedObjectBehavior obj = hit.collider.GetComponent<BedObjectBehavior>();
            if (obj != null && !obj.EstaCompletado)
            {
                objetoActual = obj;
                cerca = true;
            }
            else
            {
                cerca = false;
                objetoActual = null;
            }
        }
        else
        {
            cerca = false;
            objetoActual = null;
        }

        if (cerca && objetoActual != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                manteniendo = true;
                contadorMantener += Time.deltaTime;
                if (contadorMantener >= tiempoMantener)
                {
                    objetoActual.Interactuar();
                    contadorMantener = 0f;
                    manteniendo = false;
                    if (TodosCompletados())
                    {
                        tareaCompletada = true;
                        Debug.Log("✅ ¡Todos los objetos de la cama han sido rotados y movidos! Tarea completada.");
                    }
                }
            }
            else
            {
                manteniendo = false;
                contadorMantener = 0f;
            }
        }
    }

    bool TodosCompletados()
    {
        foreach (BedObjectBehavior obj in objetosCama)
        {
            if (!obj.EstaCompletado)
                return false;
        }
        return true;
    }

    void OnGUI()
    {
        if (cerca && objetoActual != null && !objetoActual.EstaCompletado)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.label);
            estilo.fontSize = 40;
            estilo.normal.textColor = Color.white;
            estilo.alignment = TextAnchor.MiddleCenter;

            Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);

            if (manteniendo)
            {
                float progreso = contadorMantener / tiempoMantener;
                GUI.Label(mensaje, $"Haciendo cama... {progreso * 100:F0}%", estilo);
            }
            else
            {
                string texto = objetoActual != null && objetoActual.EstaCompletado ? "" : "Mantén E para interactuar";
                if (!string.IsNullOrEmpty(texto))
                    GUI.Label(mensaje, texto, estilo);
            }
        }
    }
}
