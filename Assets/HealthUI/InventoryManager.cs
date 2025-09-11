using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject Inventory;           // kéo object InventoryMenu vào ?ây

    [Header("Key")]
    public KeyCode openKey = KeyCode.X;    // phím m?/?óng: X

    bool menuActivated = false;

    void Start()
    {
        if (Inventory) Inventory.SetActive(false);
        menuActivated = false;
        // N?u dùng FPS: khóa chu?t khi ?óng
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            menuActivated = !menuActivated;

            if (Inventory) Inventory.SetActive(menuActivated);
            Time.timeScale = menuActivated ? 0f : 1f;

            // Hi?n/?n chu?t cho ti?n thao tác UI
            Cursor.lockState = menuActivated ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = menuActivated;
        }
    }
}
