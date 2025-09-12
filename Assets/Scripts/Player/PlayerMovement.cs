using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementHN : MonoBehaviour
{
    [Header("Move / Look")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 100f;
    public Animator animator;

    [Header("Head Bobbing")]
    public Transform cameraTransform;
    public float walkBobFrequency = 4f;
    public float walkBobAmplitude = 0.1f;
    public float runBobFrequency = 7f;
    public float runBobAmplitude = 0.2f;

    [Header("Physics")]
    public float gravity = -9.81f;
    public float groundSnap = -2f;   // gi? dính ??t m??t

    [Header("Footstep (LOOP while moving)")]
    public AudioSource footSource;       // AudioSource trên Player (t? gán n?u ?? tr?ng)
    public AudioClip walkLoop;           // loop clip khi ?i (ho?c ?? null, s? fallback sang runLoop)
    public AudioClip runLoop;            // loop clip khi ch?y (ho?c ?? null, dùng walkLoop)
    [Range(0f, 1f)] public float footVolume = 1.0f;
    [Range(0.01f, 1f)] public float minMoveSpeed = 0.05f;
    [Header("Pitch")]
    public float walkPitch = 1.0f;
    public float runPitch = 1.2f;
    [Header("Fade speeds")]
    public float fadeInSpeed = 20f;
    public float fadeOutSpeed = 25f;

    // --- private ---
    CharacterController controller;
    float xRotation, bobTimer, verticalVelocity;
    Vector3 cameraInitialLocalPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
        if (cameraTransform) cameraInitialLocalPos = cameraTransform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Auto l?y AudioSource n?u quên kéo
        if (!footSource) footSource = GetComponent<AudioSource>();
        if (footSource)
        {
            footSource.playOnAwake = false;
            footSource.loop = true;   // QUAN TR?NG: ch?y d?ng loop
            footSource.spatialBlend = 0f;    // 2D ?? ch?c ch?n nghe (sau ?n mu?n 3D thì t?ng lên)
            footSource.volume = 0f;          // b?t ??u t?t ti?ng
        }
    }

    void Update()
    {
        HandleMouseLook();
        bool isRunning = HandleMovement();
        HandleHeadBob(isRunning);
        HandleFootstepLoop(isRunning);   // <-- ch? phát khi ?ang gi? phím
    }

    // ===== Look =====
    void HandleMouseLook()
    {
        if (!cameraTransform) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ===== Move (+gravity) =====
    bool HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

        // gravity cho CharacterController
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = groundSnap;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        if (animator) animator.SetBool("isWalking", move.sqrMagnitude > 0.001f);
        return isRunning;
    }

    // ===== Head Bob =====
    void HandleHeadBob(bool isRunning)
    {
        if (!cameraTransform) return;

        Vector3 vxz = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        if (vxz.magnitude > 0.1f && controller.isGrounded)
        {
            float freq = isRunning ? runBobFrequency : walkBobFrequency;
            float amp = isRunning ? runBobAmplitude : walkBobAmplitude;

            bobTimer += Time.deltaTime * freq;

            float y = Mathf.Sin(bobTimer) * amp;
            float x = Mathf.Cos(bobTimer * 0.5f) * amp * 3f;

            cameraTransform.localPosition = cameraInitialLocalPos + new Vector3(x, y, 0f);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition =
                Vector3.Lerp(cameraTransform.localPosition, cameraInitialLocalPos, Time.deltaTime * 5f);
        }
    }

    // ===== Footstep LOOP (ch? phát khi ?ang gi? input) =====
    void HandleFootstepLoop(bool isRunning)
    {
        if (!footSource) return;

        // 1) ?ang gi? input di chuy?n? (W/A/S/D, arrows, joystick)
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool pressingMove = input.sqrMagnitude > 0.01f;

        // 2) Có v?n t?c th?c s? & ?ang ch?m ??t
        Vector3 vxz = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        bool movingEnough = vxz.magnitude >= minMoveSpeed;
        bool grounded = controller.isGrounded;

        bool shouldPlay = pressingMove && movingEnough && grounded;

        if (shouldPlay)
        {
            // Ch?n clip theo ?i/ch?y (có fallback)
            AudioClip target = (isRunning && runLoop) ? runLoop : (walkLoop ? walkLoop : runLoop);
            if (footSource.clip != target)
            {
                footSource.clip = target;
                // ??i clip thì Play l?i (n?u ?ang d?ng)
                if (!footSource.isPlaying) footSource.Play();
            }
            // Pitch & Volume theo tr?ng thái
            float targetPitch = isRunning ? runPitch : walkPitch;
            float targetVolume = footVolume;

            footSource.pitch = Mathf.Lerp(footSource.pitch, targetPitch, Time.deltaTime * 10f);
            footSource.volume = Mathf.Lerp(footSource.volume, targetVolume, Time.deltaTime * fadeInSpeed);

            if (!footSource.isPlaying) footSource.Play();
        }
        else
        {
            // Không gi? phím/không grounded ? t?t ti?ng ngay (fade r?t nhanh)
            footSource.volume = Mathf.Lerp(footSource.volume, 0f, Time.deltaTime * fadeOutSpeed);
            if (footSource.isPlaying && footSource.volume < 0.01f) footSource.Stop();
        }
    }
}
