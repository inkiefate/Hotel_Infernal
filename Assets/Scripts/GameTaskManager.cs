using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTaskManager : MonoBehaviour
{
    [Header("Referencias Tareas")]
    public BedTaskManager bedTaskManager;
    public DemonBehaviour demonBehaviour;

    [Header("UI")]
    public GameObject juegoFinalizadoCanvas;  // Victoria - completar todo en <8 mins
    public GameOverUITMP interfazGameOver;    // Derrota - tiempo agotado o demonio mata

    [Header("Timers")]
    public float tiempoDemonio = 210f;        // 3.5 minutos para cama+latas
    public float tiempoGeneral = 480f;        // 8 minutos para todas las tareas

    private float tiempoRestanteDemonio;
    private float tiempoRestanteGeneral;
    private bool persecucionActivada = false;
    private bool modoMatarActivado = false;
    private bool demonioCalmado = false;
    private bool juegoGanado = false;
    private bool juegoPerdido = false;

    void Start()
    {
        // Inicializar timers
        tiempoRestanteDemonio = tiempoDemonio;
        tiempoRestanteGeneral = tiempoGeneral;

        // Ocultar UI al inicio
        if (juegoFinalizadoCanvas != null)
            juegoFinalizadoCanvas.SetActive(false);
    }

    void Update()
    {
        if (juegoGanado || juegoPerdido) return;

        // Actualizar timers
        if (!juegoGanado)
        {
            tiempoRestanteDemonio -= Time.deltaTime;
            tiempoRestanteGeneral -= Time.deltaTime;
        }

        // VERIFICAR VICTORIA: Todas las tareas completadas antes de 8 mins
        if (TodasLasTareasCompletadas() && !juegoGanado)
        {
            juegoGanado = true;
            MostrarVictoria();
            return;
        }

        // VERIFICAR DERROTA: Tiempo general agotado
        if (tiempoRestanteGeneral <= 0f && !juegoPerdido)
        {
            juegoPerdido = true;
            MostrarDerrota("Tiempo agotado");
            return;
        }

        // LÓGICA DEL DEMONIO (solo si no está calmado y no ganamos)
        if (!demonioCalmado && !juegoGanado)
        {
            VerificarTareasDemonio();
        }
    }

    void VerificarTareasDemonio()
    {
        // Verificar tareas del demonio (cama + latas)
        bool latasOk = TrashPickUp.TodasLasLatasEntregadas();
        bool camaOk = bedTaskManager != null && bedTaskManager.TareaCompletada;
        bool tareasDemonioCompletas = latasOk && camaOk;

        // Si se completan las tareas del demonio, calmarlo
        if (tareasDemonioCompletas && !demonioCalmado)
        {
            demonioCalmado = true;
            if (demonBehaviour != null)
            {
                demonBehaviour.Calmar();
            }
        }

        // Si NO se completan a tiempo, activar demonio
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

        // 2. Cama
        bool camaCompletada = bedTaskManager != null && bedTaskManager.TareaCompletada;

        // 3. Toalla
        bool toallaCompletada = false;
        ToallaPickup toalla = FindObjectOfType<ToallaPickup>();
        if (toalla != null) toallaCompletada = toalla.ToallaEntregada;

        // 4. Patito
        bool patitoCompletado = false;
        PatitoPickup patito = FindObjectOfType<PatitoPickup>();
        if (patito != null) patitoCompletado = patito.PatitoEntregado;

        // 5. Limpieza
        bool limpiezaCompletada = false;
        CleanerManager limpieza = FindObjectOfType<CleanerManager>();
        if (limpieza != null) limpiezaCompletada = limpieza.LimpiezaCompletada;

        // 6. Váteres
        bool vateresCompletados = false;
        ToiletTaskManager vateres = FindObjectOfType<ToiletTaskManager>();
        if (vateres != null)
        {
            // Necesitaríamos una propiedad pública en ToiletTaskManager
            // vateresCompletados = vateres.TareaCompletada;
            vateresCompletados = true; // Temporal - necesitas agregar la propiedad
        }

        // 7. Grifos
        bool grifosCompletados = false;
        FaucetTaskManager grifos = FindObjectOfType<FaucetTaskManager>();
        if (grifos != null)
        {
            // grifosCompletados = grifos.TareaCompletada;
            grifosCompletados = true; // Temporal
        }

        // 8. Cuadros
        bool cuadrosCompletados = false;
        FrameTaskManager cuadros = FindObjectOfType<FrameTaskManager>();
        if (cuadros != null)
        {
            // cuadrosCompletados = cuadros.TareaCompletada;
            cuadrosCompletados = true; // Temporal
        }

        // 9. Lámparas
        bool lamparasCompletadas = false;
        LampTaskManager lamparas = FindObjectOfType<LampTaskManager>();
        if (lamparas != null)
        {
            // lamparasCompletadas = lamparas.TareaCompletada;
            lamparasCompletadas = true; // Temporal
        }

        // 10. Teléfono (siempre correcto si no enfada al demonio)
        bool telefonoCompletado = true;

        // 11. Termómetro (siempre correcto si no enfada al demonio)
        bool termometroCompletado = true;

        // 12. Ventilador (siempre correcto si no enfada al demonio)
        bool ventiladorCompletado = true;

        // Devolver true solo si TODAS están completas
        return latasCompletas && camaCompletada && toallaCompletada &&
               patitoCompletado && limpiezaCompletada && vateresCompletados &&
               grifosCompletados && cuadrosCompletados && lamparasCompletadas &&
               telefonoCompletado && termometroCompletado && ventiladorCompletado;
    }

    void MostrarVictoria()
    {
        Time.timeScale = 0f;
        if (juegoFinalizadoCanvas != null)
        {
            juegoFinalizadoCanvas.SetActive(true);
        }
    }

    public void MostrarDerrota(string motivo)
    {
        juegoPerdido = true;
        Time.timeScale = 0f;
        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
        }
    }

    // Llamado por DemonBehaviour cuando mata al jugador
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

            string textoDemonio = $"Demonio: {tiempoRestanteDemonio:F0}s / 210s";
            GUI.Label(new Rect(x, y + 90, anchoBarra, 35), textoDemonio, estiloDemonio);
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
    }
}