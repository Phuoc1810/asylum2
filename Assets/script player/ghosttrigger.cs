using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    private CameraEffects cameraEffects;

    void Start()
    {
        cameraEffects = Camera.main.GetComponent<CameraEffects>();
        Debug.Log("CameraEffects g�n th�nh c�ng: " + (cameraEffects != null));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ghost"))
        {
            Debug.Log("Player gap ma !");
            cameraEffects.ZoomIn();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ghost"))
        {
            Debug.Log("Player di binh thuong!");
            cameraEffects.ResetZoom();
        }
    }
}
