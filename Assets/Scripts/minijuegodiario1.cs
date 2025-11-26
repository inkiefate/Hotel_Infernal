using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniJuegoBarraClick : MonoBehaviour
{
    public RectTransform indicador;
    public RectTransform zonaCorrecta;
    public RectTransform barra;

    public TMP_Text resultadoTexto;

    public float velocidad = 300f;
    private bool moviendoDerecha = true;
    private bool terminado = false;
    public Button taskButton;


    void Update()
    {
        if (terminado) return;

        float paso = velocidad * Time.deltaTime;
        Vector3 pos = indicador.localPosition;

        // Movimiento del indicador
        if (moviendoDerecha)
            pos.x += paso;
        else
            pos.x -= paso;

        float limite = barra.rect.width*2;

        if (pos.x >= limite)
        {
            pos.x = limite;
            moviendoDerecha = false;
        }
        if (pos.x <= -limite)
        {
            pos.x = -limite;
            moviendoDerecha = true;
        }

        indicador.localPosition = pos;

        // Detectar clic en cualquier lado
        if (Input.GetMouseButtonDown(0))
            Intentar();
    }

    void Intentar()
    {
        if (terminado) return;

        float posIndicador = indicador.localPosition.x;

        float izquierda = zonaCorrecta.localPosition.x - zonaCorrecta.rect.width / 2;
        float derecha   = zonaCorrecta.localPosition.x + zonaCorrecta.rect.width / 2;

        if (posIndicador >= izquierda && posIndicador <= derecha)
        {
            resultadoTexto.text = "¡ÉXITO!";
            resultadoTexto.color = Color.green;
             if (taskButton != null){
                taskButton.interactable = false;
        }

        }
        else
        {
            resultadoTexto.text = "FALLO";
            resultadoTexto.color = Color.red;
        }

        terminado = true;

        // Cerrar popup luego de 1.2 segundos
        Invoke("CerrarPopup", 1.2f);
    }

    void CerrarPopup()
    {
        gameObject.SetActive(false);
    }
}