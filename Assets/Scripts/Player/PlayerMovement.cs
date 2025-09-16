using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 100f;
    public Animator animator;

    [Header("Grative setting")]
    private float _velocityVertical = 0f;
    private float _gravity = -9.81f;

    public Transform cameraTransform;

    private CharacterController controller;
    private float xRotation = 0f;
    //private float bobTimer = 0f;
    private Vector3 cameraInitialPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraInitialPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveX + transform.forward * moveZ;
        Vector3 move = moveHorizontal;
        if (!controller.isGrounded)
        {
            _velocityVertical += _gravity * Time.deltaTime;
        }
        else
        {
            _velocityVertical = 0f; // Reset vertical velocity when grounded
        }
        move.y = _velocityVertical; // Apply vertical velocity for gravity
        controller.Move(move * currentSpeed * Time.deltaTime);

        bool isMoving = moveHorizontal.sqrMagnitude > 0f;
        animator.SetBool("isWalking", isMoving);
    }
}