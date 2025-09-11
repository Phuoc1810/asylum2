using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera cam;
    public float normalFOV = 60f;
    public float zoomInFOV = 40f;    // Khi nhin thay ma
    public float zoomOutFOV = 80f;   // Khi bi duoi
    public float transitionSpeed = 5f;

    private float targetFOV;

    void Start()
    {
        // Gán Camera 
        if (cam == null)
            cam = GetComponent<Camera>();

        targetFOV = normalFOV;
    }

    void Update()
    {
        // zoom FOV khi phat hien
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * transitionSpeed);
    }

    
    public void ZoomIn()
    {
        targetFOV = zoomInFOV;
    }

    // khi bi ruot
    public void ZoomOut()
    {
        targetFOV = zoomOutFOV;
    }

    
    public void ResetZoom()
    {
        targetFOV = normalFOV;
    }

    
    void DebugKeyTest()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ZoomIn();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ZoomOut();
        if (Input.GetKeyDown(KeyCode.Alpha3)) ResetZoom();
    }
}
