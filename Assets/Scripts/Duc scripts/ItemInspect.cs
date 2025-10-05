using UnityEngine;

public class ItemInspect : MonoBehaviour
{
    public Transform inspectPosition; // Empty GameObject in front of the camera
    public float rotationSpeed = 5f;
    private GameObject currentItem;
    private bool inspecting = false;

    void Update()
    {
        if (inspecting && currentItem != null)
        {
            // Rotate item with mouse drag
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;
            currentItem.transform.Rotate(Camera.main.transform.up, -rotX, Space.World);
            currentItem.transform.Rotate(Camera.main.transform.right, rotY, Space.World);

            // Exit inspection with Esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitInspection();
            }
        }
    }

    // Call this when player interacts with the closet
    public void InspectItem(GameObject itemPrefab)
    {
        if (inspecting) return;

        inspecting = true;
        currentItem = Instantiate(itemPrefab, inspectPosition.position, Quaternion.identity);
        currentItem.transform.SetParent(inspectPosition); // lock item to camera position
    }

    void ExitInspection()
    {
        inspecting = false;
        if (currentItem != null)
        {
            Destroy(currentItem);
        }
    }
}
