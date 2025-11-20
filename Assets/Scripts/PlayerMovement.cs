using UnityEngine;
using System.Collections.Generic;

// Controla el movimiento del jugador incluyendo caminar, correr, salto, doble salto y dash
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;  // Componente CharacterController del jugador
    public float speed = 5f;                 // Velocidad normal de movimiento
    public float sprintSpeed = 8f;           // Velocidad al correr (shift)
    public float gravity = -20f;             // Gravedad aplicada al jugador
    public float jump = 3.2f;                // Fuerza del salto normal
    public float doubleJump = 0.6f;          // Multiplicador para el salto doble
    public float doubleTap = 0.3f;           // Tiempo máximo entre dos pulsaciones para dash

    public float dashSpeed = 14f;            // Velocidad del dash
    public float dashDuration = 0.4f;        // Duración del dash

    private Vector3 velocity;                // Vector de velocidad vertical
    private bool enSuelo;                    // Indica si el jugador está tocando el suelo
    private int jumpCont = 0;                // Contador de saltos (para doble salto)
    private float lastJump = 0f;             // Momento del último salto

    private bool isDashing = false;          // Indica si se está realizando un dash
    private float dashTimer = 0f;            // Temporizador del dash
    private Vector3 dashDirection;           // Dirección del dash
    private KeyCode dashKey;                 // Tecla asociada al dash actual

    private Vector3 moveInput;               // Vector de entrada horizontal
    private Dictionary<KeyCode, float> lastKeyPress = new Dictionary<KeyCode, float>(); // Control de doble pulsación

    private bool llevaObjeto = false;        // Si el jugador lleva un objeto (afecta velocidad)

    // Permite activar o desactivar el estado de llevar un objeto
    public void LlevarObjeto(bool estado)
    {
        llevaObjeto = estado;
    }

    public bool EstaLlevandoObjeto => llevaObjeto;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Bloquear cursor al centro
    }

    void Update()
    {
        // Comprobar si el jugador está en el suelo
        enSuelo = controller.isGrounded;
        if (enSuelo && velocity.y < 0)
        {
            velocity.y = -2f; // Mantener ligero contacto con el suelo
            jumpCont = 0;     // Resetear contador de saltos
        }

        // Detectar doble pulsación para dash en WASD si no se lleva objeto
        if (!llevaObjeto)
        {
            DetectDash(KeyCode.W, transform.forward);
            DetectDash(KeyCode.S, -transform.forward);
            DetectDash(KeyCode.A, -transform.right);
            DetectDash(KeyCode.D, transform.right);
        }

        // Movimiento horizontal basado en WASD
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        moveInput = (transform.right * x + transform.forward * z).normalized;

        // Movimiento o dash
        if (isDashing)
        {
            if (!Input.GetKey(dashKey))
            {
                isDashing = false; // Cancelar dash si se suelta la tecla
            }
            else
            {
                controller.Move(dashDirection * dashSpeed * Time.deltaTime);
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                    isDashing = false;
            }
        }
        else
        {
            // Velocidad actual considerando sprint y si lleva objeto
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;
            if (llevaObjeto)
                currentSpeed *= 0.6f;

            controller.Move(moveInput * currentSpeed * Time.deltaTime);
        }

        // Salto y doble salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastJump = Time.time - lastJump;
            lastJump = Time.time;

            if (jumpCont < 2)
            {
                // Evitar salto doble si pasó demasiado tiempo
                if (jumpCont == 1 && timeSinceLastJump > doubleTap)
                    return;

                float jumpForce = Mathf.Sqrt(jump * -2f * gravity);
                if (jumpCont == 1)
                    jumpForce *= doubleJump; // Aplicar multiplicador de doble salto

                velocity.y = jumpForce;
                jumpCont++;
            }
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        if (velocity.y < -50f) // Limitar velocidad terminal
            velocity.y = -50f;

        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    // Detecta doble pulsación de una tecla para iniciar dash
    void DetectDash(KeyCode key, Vector3 direction)
    {
        if (Input.GetKeyDown(key))
        {
            if (lastKeyPress.ContainsKey(key) && Time.time - lastKeyPress[key] < doubleTap)
            {
                StartDash(direction, key);
            }
            lastKeyPress[key] = Time.time;
        }
    }

    // Inicia un dash en la dirección especificada
    void StartDash(Vector3 direction, KeyCode key)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = direction;
        dashKey = key;
    }
}
