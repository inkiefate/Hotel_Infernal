using UnityEngine;
using UnityEngine.UI;

public class minijuegoaire : MonoBehaviour
{
  

    public GameObject popup;        // The popup of this minigame
    public Button taskButton;       // The task button in the menu

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
          
            FinalizarMinijuego();
        }
    }

    void FinalizarMinijuego()
    {
        // Disable the task so it can't be repeated
        if (taskButton != null)
            taskButton.interactable = false;

        // Close this popup
        popup.SetActive(false);
    }
}