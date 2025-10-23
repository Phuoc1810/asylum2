using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 100f;
    public Animator animator;

    [Header("Camera Smooth")]
    [SerializeField] private float cameraSmoothTime = 0.1f;

    [Header("Breaking Effect")]
    [SerializeField] private float breathSpeed = 1f;
    [SerializeField] private float breathAmount = 0.05f;

    [Header("Grative setting")]
    private float _velocityVertical = 0f;
    private float _gravity = -9.81f;

    public Transform cameraTransform;

    private CharacterController controller;
    private float xRotation = 0f;

    private float currentXRotation = 0f;
    private float xRotationVelocity = 0f;

    private Vector3 originalCameraPos;
    private float breathTimer = 0f;

    public string footstepGroupName = "FootStep";
    [SerializeField] private float walkStepInterval = 6f;
    [SerializeField] private float runStepInterval = 5f;
    [SerializeField] private float stepTimer = 0f;
    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
        {
            originalCameraPos = cameraTransform.localPosition;
        }
    }

    void Update()
    {
        if (Cursor.visible) return;
        HandleMovement();
        HandleMouseLook();
        HandleBreathingEffect();
    }
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        currentXRotation = Mathf.SmoothDamp(
            currentXRotation,
            xRotation,
            ref xRotationVelocity,
            cameraSmoothTime
            );
        cameraTransform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
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

        HanldeFootsteps(isMoving, isRunning);
    }
    void HandleBreathingEffect()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        bool isMoving = (moveX != 0f || moveZ != 0f);

        if (!isMoving)
        {
            breathTimer += Time.deltaTime * breathSpeed;

            float breathCycle = Mathf.Sin(breathTimer);
            float breathSquared = breathCycle * breathCycle * Mathf.Sign(breathCycle);

            float swayX = Mathf.Sin(breathTimer * 1f) * breathAmount * 2f;
            float swayY = Mathf.Sin(breathTimer * 0.2f) * breathAmount * 0.08f;

            float yOffset = breathSquared * breathAmount + swayY;
            float zOffset = breathSquared * breathAmount * 0.2f;
            float xOffset = Mathf.Sin(breathTimer * 0.7f) * breathAmount * 0.1f + swayX;

            Vector3 newCameraPos = originalCameraPos;
            newCameraPos.y += yOffset;
            newCameraPos.z += zOffset;
            newCameraPos.x += xOffset;

            cameraTransform.localPosition = newCameraPos;
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                originalCameraPos,
                5 * Time.deltaTime
                );
        }
    }
    void HanldeFootsteps(bool isMoving, bool isRunning)
    {
        if (!isMoving)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0)
        {
            if (soundManager.Instance != null)
            {
                soundManager.Instance.Playsound3D(footstepGroupName, transform.position);
            }
            else
            {
                //Debug.LogWarning("SoundManager.Instance is null - no footsteps played");
            }

            stepTimer = isRunning ? runStepInterval : walkStepInterval;
        }
    }
}