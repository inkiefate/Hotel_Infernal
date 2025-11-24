using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTaskManager : MonoBehaviour
{
    [Header("Referencias Tareas")]
    public BedTaskManager bedTaskManager;
    public DemonBehaviour demonBehaviour;

    [Header("UI")]
    public GameObject juegoFinalizadoCanvas;
    public GameOverUITMP interfazGameOver;

    [Header("Timers")]
    public float tiempoDemonio = 210f;
    public float tiempoGeneral = 480f;

    private float tiempoRestanteDemonio;
    private float tiempoRestanteGeneral;
    private bool persecucionActivada = false;
    private bool modoMatarActivado = false;
    private bool demonioCalmado = false;
    private bool juegoGanado = false;
    private bool juegoPerdido = false;

    void Start()
    {
        tiempoRestanteDemonio = tiempoDemonio;
        tiempoRestanteGeneral = tiempoGeneral;

        if (juegoFinalizadoCanvas != null)
            juegoFinalizadoCanvas.SetActive(false);
    }

    void Update()
    {
        if (juegoGanado || juegoPerdido) return;

        if (!juegoGanado)
        {
            tiempoRestanteDemonio -= Time.deltaTime;
            tiempoRestanteGeneral -= Time.deltaTime;
        }

        // VERIFICAR VICTORIA
        if (TodasLasTareasCompletadas() && !juegoGanado)
        {
            juegoGanado = true;
            MostrarVictoria();
            return;
        }

        // VERIFICAR DERROTA POR TIEMPO
        if (tiempoRestanteGeneral <= 0f && !juegoPerdido)
        {
            juegoPerdido = true;
            MostrarDerrota("Tiempo agotado");
            return;
        }

        // LÓGICA DEL DEMONIO
        if (!demonioCalmado && !juegoGanado)
        {
            VerificarTareasDemonio();
        }
    }

    void VerificarTareasDemonio()
    {
        bool latasOk = TrashPickUp.TodasLasLatasEntregadas();
        bool camaOk = bedTaskManager != null && bedTaskManager.TareaCompletada;
        bool tareasDemonioCompletas = latasOk && camaOk;

        if (tareasDemonioCompletas && !demonioCalmado)
        {
            demonioCalmado = true;
            if (demonBehaviour != null)
            {
                demonBehaviour.Calmar();
            }
        }

        if (!tareasDemonioCompletas && !demonioCalmado)
        {
            if (tiempoRestanteDemonio <= 60f && !persecucionActivada)
            {
                demonBehaviour.ActivarPersecucionSuave();
                persecucionActivada = true;
            }

            if (tiempoRestanteDemonio <= 0f && !modoMatarActivado)
            {
                demonBehaviour.ActivarModoMatar();
                modoMatarActivado = true;
            }
        }
    }

    bool TodasLasTareasCompletadas()
    {
        // 1. Latas (5)
        bool latasCompletas = TrashPickUp.TodasLasLatasEntregadas();
        if (!latasCompletas) return false;

        // 2. Cama
        bool camaCompletada = bedTaskManager != null && bedTaskManager.TareaCompletada;
        if (!camaCompletada) return false;

        // 3. Toalla
        bool toallaCompletada = false;
        ToallaPickup toalla = FindObjectOfType<ToallaPickup>();
        if (toalla != null) toallaCompletada = toalla.ToallaEntregada;
        if (!toallaCompletada) return false;

        // 4. Patito
        bool patitoCompletado = false;
        PatitoPickup patito = FindObjectOfType<PatitoPickup>();
        if (patito != null) patitoCompletado = patito.PatitoEntregado;
        if (!patitoCompletado) return false;

        // 5. Limpieza
        bool limpiezaCompletada = false;
        CleanerManager limpieza = FindObjectOfType<CleanerManager>();
        if (limpieza != null) limpiezaCompletada = limpieza.LimpiezaCompletada;
        if (!limpiezaCompletada) return false;

        // 6. Váteres
        bool vateresCompletados = false;
        ToiletTaskManager vateres = FindObjectOfType<ToiletTaskManager>();
        if (vateres != null) vateresCompletados = vateres.TareaCompletada;
        if (!vateresCompletados) return false;

        // 7. Grifos
        bool grifosCompletados = false;
        FaucetTaskManager grifos = FindObjectOfType<FaucetTaskManager>();
        if (grifos != null) grifosCompletados = grifos.TareaCompletada;
        if (!grifosCompletados) return false;

        // 8. Cuadros
        bool cuadrosCompletados = false;
        FrameTaskManager cuadros = FindObjectOfType<FrameTaskManager>();
        if (cuadros != null) cuadrosCompletados = cuadros.TareaCompletada;
        if (!cuadrosCompletados) return false;

        // 9. Lámparas
        bool lamparasCompletadas = false;
        LampTaskManager lamparas = FindObjectOfType<LampTaskManager>();
        if (lamparas != null) lamparasCompletadas = lamparas.TareaCompletada;
        if (!lamparasCompletadas) return false;

        // 10. Teléfono
        bool telefonoCompletado = false;
        TelefonoInteract telefono = FindObjectOfType<TelefonoInteract>();
        if (telefono != null) telefonoCompletado = telefono.TareaCompletada;
        if (!telefonoCompletado) return false;

        // 11. Termómetro
        bool termometroCompletado = false;
        TermometroInteract termometro = FindObjectOfType<TermometroInteract>();
        if (termometro != null) termometroCompletado = termometro.TareaCompletada;
        if (!termometroCompletado) return false;

        // 12. Ventilador
        bool ventiladorCompletado = false;
        VentiladorInteract ventilador = FindObjectOfType<VentiladorInteract>();
        if (ventilador != null) ventiladorCompletado = ventilador.TareaCompletada;
        if (!ventiladorCompletado) return false;

        return true;
    }

    // NUEVO MÉTODO: Contar tareas completadas
    int ContarTareasCompletadas()
    {
        int contador = 0;

        try
        {
            // 1. Latas (5) - cuenta como 1 tarea cuando todas están entregadas
            if (TrashPickUp.TodasLasLatasEntregadas())
            {
                contador++;
            }

            // 2. Cama
            if (bedTaskManager != null && bedTaskManager.TareaCompletada)
            {
                contador++;
            }

            // 3. Toalla
            ToallaPickup toalla = FindObjectOfType<ToallaPickup>();
            if (toalla != null && toalla.ToallaEntregada)
            {
                contador++;
            }

            // 4. Patito
            PatitoPickup patito = FindObjectOfType<PatitoPickup>();
            if (patito != null && patito.PatitoEntregado)
            {
                contador++;
            }

            // 5. Limpieza
            CleanerManager limpieza = FindObjectOfType<CleanerManager>();
            if (limpieza != null && limpieza.LimpiezaCompletada)
            {
                contador++;
            }

            // 6. Váteres
            ToiletTaskManager vateres = FindObjectOfType<ToiletTaskManager>();
            if (vateres != null && vateres.TareaCompletada)
            {
                contador++;
            }

            // 7. Grifos
            FaucetTaskManager grifos = FindObjectOfType<FaucetTaskManager>();
            if (grifos != null && grifos.TareaCompletada)
            {
                contador++;
            }

            // 8. Cuadros
            FrameTaskManager cuadros = FindObjectOfType<FrameTaskManager>();
            if (cuadros != null && cuadros.TareaCompletada)
            {
                contador++;
            }

            // 9. Lámparas
            LampTaskManager lamparas = FindObjectOfType<LampTaskManager>();
            if (lamparas != null && lamparas.TareaCompletada)
            {
                contador++;
            }

            // 10. Teléfono
            TelefonoInteract telefono = FindObjectOfType<TelefonoInteract>();
            if (telefono != null && telefono.TareaCompletada)
            {
                contador++;
            }

            // 11. Termómetro
            TermometroInteract termometro = FindObjectOfType<TermometroInteract>();
            if (termometro != null && termometro.TareaCompletada)
            {
                contador++;
            }

            // 12. Ventilador
            VentiladorInteract ventilador = FindObjectOfType<VentiladorInteract>();
            if (ventilador != null && ventilador.TareaCompletada)
            {
                contador++;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error contando tareas: " + e.Message);
        }

        return contador;
    }

    void MostrarVictoria()
    {
        Time.timeScale = 0f;
        if (juegoFinalizadoCanvas != null)
        {
            juegoFinalizadoCanvas.SetActive(true);
        }
        Debug.Log("¡VICTORIA! Todas las tareas completadas.");
    }

    public void MostrarDerrota(string motivo)
    {
        juegoPerdido = true;
        Time.timeScale = 0f;
        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
        }
        Debug.Log("DERROTA: " + motivo);
    }

    public void JugadorMuertoPorDemonio()
    {
        if (!juegoGanado && !juegoPerdido)
        {
            MostrarDerrota("El demonio te atrapó");
        }
    }

    void OnGUI()
    {
        if (Time.timeScale == 0f) return;

        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = 34;
        estiloTexto.normal.textColor = Color.white;
        estiloTexto.alignment = TextAnchor.UpperLeft;

        float anchoBarra = 520f;
        float altoBarra = 45f;
        float x = 35f;
        float y = 55f;

        // CONTADOR DE TAREAS - ARRIBA EN EL CENTRO
        int tareasCompletadas = ContarTareasCompletadas();
        int totalTareas = 12;

        GUIStyle estiloContador = new GUIStyle(GUI.skin.label);
        estiloContador.fontSize = 36;
        estiloContador.normal.textColor = Color.yellow;
        estiloContador.alignment = TextAnchor.UpperCenter;
        estiloContador.fontStyle = FontStyle.Bold;

        string textoContador = $"Tareas: {tareasCompletadas}/{totalTareas}";
        GUI.Label(new Rect(Screen.width / 2 - 100, 20, 200, 50), textoContador, estiloContador);

        // BARRA TIEMPO GENERAL (8 minutos) - AZUL
        GUI.color = Color.gray;
        GUI.DrawTexture(new Rect(x, y, anchoBarra, altoBarra), Texture2D.whiteTexture);

        float porcentajeGeneral = tiempoRestanteGeneral / tiempoGeneral;
        GUI.color = juegoGanado ? Color.green : Color.cyan;
        GUI.DrawTexture(new Rect(x, y, anchoBarra * porcentajeGeneral, altoBarra), Texture2D.whiteTexture);

        string textoGeneral = juegoGanado ? "¡VICTORIA!" : $"Tiempo total: {tiempoRestanteGeneral:F0}s";
        GUI.color = juegoGanado ? Color.green : Color.white;
        GUI.Label(new Rect(x, y - 45, anchoBarra, 45), textoGeneral, estiloTexto);

        // BARRA TIEMPO DEMONIO (3.5 minutos) - NARANJA/ROJO
        if (!demonioCalmado && !juegoGanado)
        {
            float porcentajeDemonio = tiempoRestanteDemonio / tiempoDemonio;

            // Fondo de la barra del demonio
            GUI.color = Color.gray;
            GUI.DrawTexture(new Rect(x, y + 60, anchoBarra, 25), Texture2D.whiteTexture);

            // Barra de progreso del demonio - COLOR NARANJA que cambia a ROJO
            if (modoMatarActivado)
            {
                GUI.color = Color.red; // ROJO en modo matar
            }
            else if (persecucionActivada)
            {
                GUI.color = new Color(1f, 0.5f, 0f); // NARANJA intenso cuando está enfadado
            }
            else
            {
                GUI.color = new Color(1f, 0.7f, 0.2f); // NARANJA más suave cuando está tranquilo
            }

            GUI.DrawTexture(new Rect(x, y + 60, anchoBarra * porcentajeDemonio, 25), Texture2D.whiteTexture);

            // Texto del timer del demonio - MÁS GRANDE Y CLARO
            GUIStyle estiloDemonio = new GUIStyle(GUI.skin.label);
            estiloDemonio.fontSize = 28;
            estiloDemonio.normal.textColor = Color.white;
            estiloDemonio.alignment = TextAnchor.UpperLeft;
            estiloDemonio.fontStyle = FontStyle.Bold;

            if (tiempoRestanteDemonio > 0)
            {
                string textoDemonio = $"Demonio: {tiempoRestanteDemonio:F0}s / 210s";
                GUI.Label(new Rect(x, y + 90, anchoBarra, 35), textoDemonio, estiloDemonio);
            }
        }

        // ESTADO DEL DEMONIO
        string estadoDemonio = demonioCalmado ? "DEMONIO: CALMADO" :
                              (modoMatarActivado ? "DEMONIO: MODO MATAR" :
                              (persecucionActivada ? "DEMONIO: ENFADADO" : "DEMONIO: TRANQUILO"));
        GUI.Label(new Rect(x, y + 130, anchoBarra, 45), estadoDemonio, estiloTexto);

        // MENSAJE DE VICTORIA
        if (juegoGanado)
        {
            GUI.Label(new Rect(x, y + 180, anchoBarra, 45), "TODAS LAS TAREAS COMPLETADAS", estiloTexto);
        }

        // DEBUG: Mostrar estado cada cierto tiempo
        if (Time.frameCount % 300 == 0) // Cada ~5 segundos
        {
            Debug.Log($"Contador tareas: {tareasCompletadas}/12 - Tiempo general: {tiempoRestanteGeneral:F0}s - Tiempo demonio: {tiempoRestanteDemonio:F0}s");
        }
    }
}