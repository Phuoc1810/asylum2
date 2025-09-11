using UnityEngine;

public class PlayerMovementHN : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 100f;
    public Animator animator;

    [Header("Head Bobbing Settings")]
    public Transform cameraTransform;
    public float walkBobFrequency = 4f;
    public float walkBobAmplitude = 0.1f;
    public float runBobFrequency = 7f;
    public float runBobAmplitude = 0.2f;

    private CharacterController controller;
    private float xRotation = 0f;
    private float bobTimer = 0f;
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
        HandleHeadBob();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        bool isMoving = move.sqrMagnitude > 0f;
        if (animator) animator.SetBool("isWalking", isMoving);
    }

    void HandleHeadBob()
    {
        Vector3 velocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        if (velocity.magnitude > 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float frequency = isRunning ? runBobFrequency : walkBobFrequency;
            float amplitude = isRunning ? runBobAmplitude : walkBobAmplitude;

            bobTimer += Time.deltaTime * frequency;

            float bobOffsetY = Mathf.Sin(bobTimer) * amplitude;
            float swayAmplitude = amplitude * 3f;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * swayAmplitude;

            cameraTransform.localPosition = cameraInitialPosition + new Vector3(bobOffsetX, bobOffsetY, 0);
        }
        else
        {
            bobTimer = 0;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraInitialPosition,
                Time.deltaTime * 5f
            );
        }
    }
}
