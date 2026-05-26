using UnityEngine;

/// <summary>
/// Controlador de movimiento para RPG 3D en tercera persona.
/// Requiere: CharacterController, Animator en el mismo GameObject.
/// Compatible con Unity 6000.3.11f1
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 12f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [Header("Cámara")]
    [SerializeField] private Transform cameraTransform;

    // Componentes
    private CharacterController controller;
    private Animator animator;

    // Estado interno
    private Vector3 velocity;
    private Vector3 moveDirection;
    private float currentSpeed;
    private bool isGrounded;
    private bool wasGrounded;

    // Hashes de parámetros del Animator (optimización)
    private static readonly int HashSpeed = Animator.StringToHash("Speed");
    private static readonly int HashIsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int HashJump = Animator.StringToHash("Jump");
    private static readonly int HashFreeFall = Animator.StringToHash("FreeFall");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Si no se asignó la cámara, usar la cámara principal
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Si no se asignó el groundCheck, usar la posición del personaje
        if (groundCheck == null)
            groundCheck = transform;
    }

    private void Update()
    {
        CheckGrounded();
        HandleGravity();
        HandleMovement();
        UpdateAnimator();
    }

    // ??????????????????????????????????????????
    //  DETECCIÓN DE SUELO
    // ??????????????????????????????????????????
    private void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        // Al aterrizar, limpiar velocidad vertical residual
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
    }

    // ??????????????????????????????????????????
    //  GRAVEDAD Y SALTO
    // ??????????????????????????????????????????
    private void HandleGravity()
    {
        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(HashJump);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ??????????????????????????????????????????
    //  MOVIMIENTO HORIZONTAL
    // ??????????????????????????????????????????
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        bool isRunning = Input.GetButton("Fire3"); // Shift
        float targetSpeed = inputDir.magnitude > 0.1f
            ? (isRunning ? runSpeed : walkSpeed)
            : 0f;

        // Suavizar velocidad
        float smoothTime = inputDir.magnitude > 0.1f ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, smoothTime * Time.deltaTime);

        if (inputDir.magnitude > 0.1f)
        {
            // Dirección relativa a la cámara
            Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            moveDirection = (camForward * inputDir.z + camRight * inputDir.x).normalized;

            // Rotar el personaje hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    // ??????????????????????????????????????????
    //  ANIMATOR
    // ??????????????????????????????????????????
    private void UpdateAnimator()
    {
        float normalizedSpeed = currentSpeed / runSpeed;
        animator.SetFloat(HashSpeed, normalizedSpeed, 0.1f, Time.deltaTime);
        animator.SetBool(HashIsGrounded, isGrounded);
        animator.SetBool(HashFreeFall, !isGrounded && velocity.y < -3f);
    }

    // ??????????????????????????????????????????
    //  GIZMOS (Editor)
    // ??????????????????????????????????????????
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}