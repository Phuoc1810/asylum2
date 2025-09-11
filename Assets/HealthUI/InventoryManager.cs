using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject Inventory;           // k�o object InventoryMenu v�o ?�y

    [Header("Key")]
    public KeyCode openKey = KeyCode.X;    // ph�m m?/?�ng: X

    bool menuActivated = false;

    void Start()
    {
        if (Inventory) Inventory.SetActive(false);
        menuActivated = false;
        // N?u d�ng FPS: kh�a chu?t khi ?�ng
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

            // Hi?n/?n chu?t cho ti?n thao t�c UI
            Cursor.lockState = menuActivated ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = menuActivated;
        }
    }
}
