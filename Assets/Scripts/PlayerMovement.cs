using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    [Header("Player Components")]
    public CharacterController controller;
    public Animator animator;

    [Header("Key Bindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode danceKey = KeyCode.F;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private float originalHeight;
    public float crouchHeight = 1f;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        originalHeight = controller.height;
    }

    void Update()
    {
        CheckGrounded();
        Move();

        animator = GetComponent<Animator>();
    }

    void CheckGrounded()
    {
        // Raycast downwards to check for ground layer
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // Reset velocity if grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void Move()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Determine speed
        float speed = walkSpeed;
        if (Input.GetKey(runKey) && !isCrouching)
        {
            speed = runSpeed;
           
        }
        else if (isCrouching)
        {
            speed = crouchSpeed;
        }

        // Apply movement
        controller.Move(move * speed * Time.deltaTime);

        // Handle jumping
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Handle crouching
        if (Input.GetKeyDown(crouchKey))
        {
            ToggleCrouch();
        }

        // Handle dancing
        if (Input.GetKeyDown(danceKey))
        {
            animator.SetTrigger("Dance");
        }

        // Update animations
        animator.SetFloat("Speed", move.magnitude);
        animator.SetBool("IsRunning", Input.GetKey(runKey) && !isCrouching);
        animator.SetBool("IsCrouching", isCrouching);
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        controller.height = isCrouching ? crouchHeight : originalHeight;
    }
}