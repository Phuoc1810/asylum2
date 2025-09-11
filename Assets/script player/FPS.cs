using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;   
#endif

public class MouseLook : MonoBehaviour
{
    [Header("Sensitivity")]
    [Tooltip("Do nhay chuot")]
    public float mouseSensitivity = 100f;

    [Header("References")]
    [Tooltip("Transform c?a Player (object có CharacterController).")]
    public Transform playerBody;          //  gán trong Inspector

    float pitch = 0f;                     // xoay doc nghieng cuoi

    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        if (playerBody == null)
            playerBody = transform.parent;        // camera là child cua player
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM   // (Input System package )
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;
#else                     // (Input Manager )
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
#endif

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);    

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}
