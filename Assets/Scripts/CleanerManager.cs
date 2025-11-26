using UnityEngine;
using System.Collections.Generic;

public class CleanerManager : MonoBehaviour
{
    public float tiempoLimite = 80f;
    public float tiempoReinicio = 60f;
    public Camera playerCamera;
    public GameObject[] cubosLimpiables;

    private Dictionary<GameObject, int> interacciones = new Dictionary<GameObject, int>();
    private float temporizador;
    private float tiempoEspera;
    private bool limpiezaActiva = true;
    private GameObject cuboActual;
    private bool cerca = false;

    void Start()
    {
        temporizador = tiempoLimite;

        foreach (GameObject cubo in cubosLimpiables)
        {
            cubo.SetActive(true);
            interacciones[cubo] = 0;

            if (!cubo.TryGetComponent(out CleanerBehavior behavior))
                cubo.AddComponent<CleanerBehavior>();
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
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

        if (limpiezaActiva)
        {
            temporizador -= Time.deltaTime;

            if (cerca && Input.GetKeyDown(KeyCode.E) && cuboActual != null)
            {
                if (interacciones.ContainsKey(cuboActual))
                {
                    interacciones[cuboActual]++;
                    Debug.Log($"Cubo {cuboActual.name}: {interacciones[cuboActual]} interacciones.");

                    if (interacciones[cuboActual] >= 3)
                    {
                        cuboActual.SetActive(false);
                        interacciones[cuboActual] = 0;
                    }

                    if (TodosCubosDesactivados())
                    {
                        limpiezaActiva = false;
                        tiempoEspera = tiempoReinicio;
                        Debug.Log("¡Todos los cubos han sido limpiados!");
                    }
                }
            }

            if (temporizador <= 0f)
            {
                limpiezaActiva = false;
                tiempoEspera = tiempoReinicio;
                Debug.Log("¡Tiempo agotado! Los cubos reaparecerán en 1 minuto.");
            }
        }
        else
        {
            tiempoEspera -= Time.deltaTime;

            if (tiempoEspera <= 0f)
            {
                ReiniciarCiclo();
            }
        }
    }

    bool TodosCubosDesactivados()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            if (cubo.activeSelf)
                return false;
        }
        return true;
    }

    void ReiniciarCiclo()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            cubo.SetActive(true);
            interacciones[cubo] = 0;
        }

        temporizador = tiempoLimite;
        limpiezaActiva = true;
        Debug.Log("¡Los cubos han reaparecido! Comienza un nuevo ciclo.");
    }

    void OnGUI()
{
    GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
    estiloTexto.fontSize = 20;
    estiloTexto.normal.textColor = Color.white;
    estiloTexto.alignment = TextAnchor.UpperCenter;

    float anchoBarra = 300f;
    float altoBarra = 25f;
    float x = Screen.width / 2 - anchoBarra / 2;
    float y = 60f;

    // Fondo de la barra
    GUI.color = Color.gray;
    GUI.DrawTexture(new Rect(x, y, anchoBarra, altoBarra), Texture2D.whiteTexture);

    // Barra de progreso
    float porcentaje = limpiezaActiva ? temporizador / tiempoLimite : tiempoEspera / tiempoReinicio;
    GUI.color = limpiezaActiva ? Color.green : Color.red;
    GUI.DrawTexture(new Rect(x, y, anchoBarra * porcentaje, altoBarra), Texture2D.whiteTexture);

    // Texto encima de la barra
    string texto = limpiezaActiva
        ? $"Tiempo restante: {temporizador:F1}s"
        : $"Reinicio en: {tiempoEspera:F1}s";

    GUI.color = Color.white;
    GUI.Label(new Rect(x, y - 30, anchoBarra, 30), texto, estiloTexto);

    // Mensaje de interacción
    if (cerca && limpiezaActiva)
    {
        Rect mensaje = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
        GUI.Label(mensaje, "Presiona E para limpiar cubo", estiloTexto);
    }
}
}

