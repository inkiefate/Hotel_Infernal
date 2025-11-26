using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PanelNoche : MonoBehaviour
{
    public GameObject popup;   // The night panel popup

    public void CerrarPopup()
    {
        popup.SetActive(false);
    }

    public void IrAlHotel()
    {
        SceneManager.LoadScene("Hotel");
    }
}