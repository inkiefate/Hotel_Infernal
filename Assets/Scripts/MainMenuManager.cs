using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject optionsCanvas;
    public GameObject controlesCanvas;

    public void PlayGame()
    {
        SceneManager.LoadScene("NombreDeTuEscena"); 
    }

    public void OpenOptions()
    {
        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void OpenControles()
    {
        mainMenuCanvas.SetActive(false);
        controlesCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        optionsCanvas.SetActive(false);
        controlesCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}

