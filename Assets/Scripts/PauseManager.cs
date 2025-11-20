using UnityEngine;
using UnityEngine.SceneManagement;

// Maneja la pausa del juego, menú de pausa y navegación hacia opciones o menú principal
public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Canvas del menú de pausa
    private bool isPaused = false;  // Indica si el juego está pausado
    public GameObject menuCanvas;   // Canvas de opciones dentro del menú de pausa

    void Start()
    {
        // Al iniciar, ocultar menús y bloquear cursor
        pauseMenuUI.SetActive(false); 
        menuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Detectar tecla Escape para alternar pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // Reanuda el juego ocultando el menú de pausa y desbloqueando el cursor
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Pausa el juego mostrando el menú y desbloqueando el cursor
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Reinicia la escena actual
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Abre el menú de opciones desde el menú de pausa
    public void OpenOptions()
    {
        Cursor.lockState = CursorLockMode.None;  
        Cursor.visible = true;
        pauseMenuUI.SetActive(false);     
        menuCanvas.SetActive(true);     
    }

    // Cierra el menú de opciones y vuelve al menú de pausa
    public void CloseOptions()
    {
        menuCanvas.SetActive(false);     
        pauseMenuUI.SetActive(true);     
    }

    // Vuelve al menú principal desde el juego pausado
    public void backToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenuControlador");
    }
}
