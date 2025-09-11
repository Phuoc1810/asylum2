using UnityEngine;


public class CameraBobbing : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;         

    [Header("Bobbing ? len / xuong")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 13f;
    public float runBobAmount = 0.08f;
    public float crouchBobAmount = 0.02f;

    [Header("Nghieng trai phai")]
    public float tiltAmount = 2f;    
    public float tiltSpeed = 8f;    

    Vector3 localOrigin;         
    Quaternion rotOrigin;        
    float bobTimer;

    void Awake()
    {
        localOrigin = transform.localPosition;
        rotOrigin = transform.localRotation;

        
        if (controller == null)
        {
            controller = GetComponentInParent<CharacterController>();
            if (controller == null)
                Debug.LogWarning("CameraBobbing: Khong tim thay CharacterController!");
        }
    }

    
    void LateUpdate()
    {
        if (controller == null) return;

        Vector3 velocity = controller.velocity;
        bool isGround = controller.isGrounded;
        bool isMoving = velocity.magnitude > 0.1f && isGround;

        if (isMoving)
        {
            float bobSpeed = (velocity.z > controller.height) ? runBobSpeed : walkBobSpeed;
            float bobAmount = (velocity.z > controller.height) ? runBobAmount : walkBobAmount;
            if (controller.height < 1.5f) bobAmount = crouchBobAmount;

            bobTimer += Time.deltaTime * bobSpeed;
            float offsetY = Mathf.Sin(bobTimer) * bobAmount;

            transform.localPosition = new Vector3(localOrigin.x,
                                                  localOrigin.y + offsetY,
                                                  localOrigin.z);
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition,
                                                    localOrigin,
                                                    Time.deltaTime * walkBobSpeed);
        }

        float targetTilt = -velocity.x * tiltAmount;           
        Quaternion tiltRot = Quaternion.Euler(0f, 0f, targetTilt);

        transform.localRotation = Quaternion.Slerp(transform.localRotation,
                                                   rotOrigin * tiltRot,
                                                   Time.deltaTime * tiltSpeed);
    }
}
