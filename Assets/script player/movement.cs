using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.5f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRun = 20f;  
    public float staminaRegen = 10f; 

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;

    [Header("References")]
    public UnityEngine.UI.Slider staminaSlider;   // gán Slider 

    CharacterController controller;
    Vector3 velocity;
    float stamina;
    bool isCrouching;
    bool isRunning;

    const float gravity = -9.81f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = stamina;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleStamina();
        UpdateUI();
    }

    #region Movement
    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        /* Toggle crouch bang phím Space */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : standHeight;
        }

        /*Chay khi Shift +  stamina + không crouch */
        bool wantsRun = Input.GetKey(KeyCode.LeftShift) &&
                        z > 0.1f &&
                        !isCrouching &&
                        stamina > 0f;

        isRunning = wantsRun;

        float currentSpeed = isCrouching ? crouchSpeed :
                             (isRunning ? runSpeed : walkSpeed);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region Stamina
    void HandleStamina()
    {
        if (isRunning)
            stamina -= staminaDrainRun * Time.deltaTime;
        else
            stamina += staminaRegen * Time.deltaTime;

        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        if (stamina <= 0f) isRunning = false;   // het stamina khi dang chay 
    }
    #endregion

    
    #region UI
    void UpdateUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = stamina;
    }
    #endregion
}
