using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimientoRPG : MonoBehaviour
{
    private CharacterController controller;
    private Transform camaraPrincipal;

    [Header("Ajustes de Movimiento")]
    public float velocidad = 6.0f;
    public float suavizadoGiro = 0.1f;
    private float velocidadGiro;

    [Header("Físicas y Salto")]
    public float gravedad = -15f;
    public float alturaSalto = 1.2f; // Altura en metros que alcanzará el personaje
    private Vector3 velocidadVertical;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camaraPrincipal = Camera.main.transform;

        // Bloquea el cursor en el centro y lo oculta
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Inputs de Movimiento (WASD / Flechas)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direccion = new Vector3(h, 0f, v).normalized;

        if (direccion.magnitude >= 0.1f)
        {
            // 2. Calcular ángulo basado en la posición de la cámara
            float anguloObjetivo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg + camaraPrincipal.eulerAngles.y;
            float anguloSuave = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloObjetivo, ref velocidadGiro, suavizadoGiro);

            // Rotamos el personaje
            transform.rotation = Quaternion.Euler(0f, anguloSuave, 0f);

            // 3. Dirección del movimiento
            Vector3 dirMovimiento = Quaternion.Euler(0f, anguloObjetivo, 0f) * Vector3.forward;
            controller.Move(dirMovimiento.normalized * velocidad * Time.deltaTime);
        }

        // 4. Control de Gravedad y Salto
        if (controller.isGrounded)
        {
            if (velocidadVertical.y < 0)
            {
                velocidadVertical.y = -2f; // Mantiene al personaje pegado al suelo
            }

            // NUEVO: Detectar el salto solo si estamos tocando el suelo
            if (Input.GetButtonDown("Jump")) // "Jump" es la barra espaciadora por defecto
            {
                // Fórmula física para calcular la velocidad inicial requerida para alcanzar 'alturaSalto'
                velocidadVertical.y = Mathf.Sqrt(alturaSalto * -2f * gravedad);
            }
        }

        // Aplicar la aceleración de la gravedad de forma continua
        velocidadVertical.y += gravedad * Time.deltaTime;

        // Mover al personaje en el eje vertical (caída o subida del salto)
        controller.Move(velocidadVertical * Time.deltaTime);

        // EXTRA: Si presionas ESC, liberas el ratón
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}