using UnityEngine;

// Se asegura de que haya un SettingsManager en la escena al iniciar el juego
public class SettingsBootstrap : MonoBehaviour
{
    void Awake()
    {
        // Verificar si ya existe un SettingsManager
        if (SettingsManager.Instance == null)
        {
            // Cargar el prefab del SettingsManager desde la carpeta Resources/Managers
            GameObject prefab = Resources.Load<GameObject>("Managers/SettingsManager");

            if (prefab != null)
            {
                // Instanciar el prefab si se encontró
                Instantiate(prefab);
                Debug.Log("SettingsManager instanciado");
            }
            else
            {
                // Mostrar error si no se encuentra el prefab
                Debug.LogError("Prefab SettingsManager no encontrado");
            }
        }
    }
}



