using UnityEngine;

// Comportamiento para un objeto "Cleaner"
public class CleanerBehavior : MonoBehaviour
{
    // Referencia al componente Renderer para cambiar el color del objeto
    private Renderer rend;

    public Material materialSucio;
    void Start()
    {
        // Obtiene el Renderer del objeto al iniciar
        rend = GetComponent<Renderer>();

        // Si el Renderer existe, cambia el color del material a verde
        if (rend != null)
            rend.material = materialSucio;
    }

    void OnEnable()
    {
        // Cada vez que el objeto se habilita, vuelve a poner el color verde
        if (rend != null)
            rend.material = materialSucio;
    }

    void OnDisable()
    {
        // Muestra un mensaje en consola indicando que el objeto fue deshabilitado o "limpiado"
        Debug.Log(gameObject.name + " ha sido limpiado.");
    }
}


