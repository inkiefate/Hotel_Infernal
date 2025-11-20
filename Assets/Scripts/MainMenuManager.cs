using UnityEngine;
using UnityEngine.SceneManagement;

// Maneja el menú principal, opciones y controles
public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;   // Canvas del menú principal
    public GameObject optionsCanvas;    // Canvas de opciones
    public GameObject controlesCanvas;  // Canvas de controles

    // Inicialización al iniciar el juego
    public void Start()
    {
        // Mostrar el cursor y desbloquearlo
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Mostrar solo el menú principal al inicio
        optionsCanvas.SetActive(false);
        controlesCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    // Comenzar el juego cargando la escena principal
    public void PlayGame()
    {
        Time.timeScale = 1f;               // Asegurar que el tiempo esté activo
        Cursor.lockState = CursorLockMode.Locked; // Bloquear y ocultar cursor
        Cursor.visible = false;

        SceneManager.LoadScene("Hotel");   // Cargar escena principal
    } 

    // Abrir el menú de opciones
    public void OpenOptions()
    {
        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    // Abrir el menú de controles
    public void OpenControles()
    {
        mainMenuCanvas.SetActive(false);
        controlesCanvas.SetActive(true);
    }

    // Volver al menú principal desde opciones o controles
    public void BackToMainMenu()
    {
        optionsCanvas.SetActive(false);
        controlesCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    // Ajustar sensibilidad del mouse usando el SettingsManager
    public void SetSensibilidad(float value)
    {
        SettingsManager.Instance.SetSensibilidad(value);
    }

    // Ajustar volumen del juego usando el SettingsManager
    public void SetVolumen(float value)
    {
        SettingsManager.Instance.SetVolumen(value);
    }

}
