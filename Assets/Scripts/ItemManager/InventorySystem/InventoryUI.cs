using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel; // Toàn bộ inventory panel
    [SerializeField] private GameObject aimPointPanel; // Aim point UI
    [SerializeField] private Transform panelBody; // Panel Body thay cho itemListContent
    [SerializeField] private RawImage previewImage; // Preview render texture

    [Header("Item Entry Prefab")]
    [SerializeField] private GameObject itemEntryPrefab; // Prefab của InventoryItemEntry

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HeadBobbingController headBobbingController;

    [Header("Preview Settings")]
    [SerializeField] private Camera previewCamera;
    [SerializeField] private Transform previewSpawnPoint;

    [Header("Layout Settings")]
    [SerializeField] private float itemHeight = 50f; // Chiều cao mỗi item
    [SerializeField] private float spacing = 5f; // Khoảng cách giữa items

    private bool isOpen = false;
    private GameObject currentPreviewObject;
    private InventoryItemEntry selectedEntry;

    private static Camera persistentPreviewCamera;

    void Start()
    {
        InitializeUI();
        SubscribeToInventory();
    }

    private void OnDestroy()
    {
        UnsubscribeFromInventory();
    }

    void Update()
    {
        HandleInput();
    }

    private void InitializeUI()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        if (previewCamera != null)
        {
            if (persistentPreviewCamera == null)
            {
                persistentPreviewCamera = previewCamera;
                DontDestroyOnLoad(previewCamera.gameObject);
            }
            else if (previewCamera != persistentPreviewCamera)
            {
                Destroy(previewCamera.gameObject);
                previewCamera = persistentPreviewCamera;
            }
        }
    }

    private void SubscribeToInventory()
    {
        if (InventoryService.Instance != null)
        {
            InventoryService.Instance.Changed += OnInventoryChanged;
        }
    }

    private void UnsubscribeFromInventory()
    {
        if (InventoryService.Instance != null)
        {
            InventoryService.Instance.Changed -= OnInventoryChanged;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
        if (isOpen && Input.GetKeyDown(KeyCode.Q))
        {
            CloseInventory();
        }
    }

    private void ToggleInventory()
    {
        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        isOpen = true;
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }
        DisablePlayerControls();
        RefreshItemList();
    }

    private void CloseInventory()
    {
        isOpen = false;
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        EnablePlayerControls();
        ClearPreview();
    }

    private void DisablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (headBobbingController != null)
            headBobbingController.enabled = false;

        if (aimPointPanel != null)
            aimPointPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EnablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (headBobbingController != null)
            headBobbingController.enabled = true;

        if (aimPointPanel != null)
            aimPointPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnInventoryChanged()
    {
        if (isOpen)
        {
            RefreshItemList();
        }
    }

    private void RefreshItemList()
    {
        ClearItemList();

        if (InventoryService.Instance == null)
        {
            return;
        }

        var items = InventoryService.Instance.SnapShot();

        float yOffset = 0f; // Bắt đầu từ top của Panel Body
        foreach (var kvp in items)
        {
            CreateItemEntry(kvp.Key, kvp.Value, ref yOffset);
        }
    }
    private void ClearItemList()
    {
        if (panelBody == null)
        {
            return;
        }

        foreach (Transform child in panelBody)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateItemEntry(string itemId, int quantity, ref float yOffset)
    {
        if (itemEntryPrefab == null || panelBody == null)
        {
            return;
        }

        GameObject entry = Instantiate(itemEntryPrefab, panelBody);
        entry.layer = LayerMask.NameToLayer("UI"); // Đảm bảo layer UI

        RectTransform entryRect = entry.GetComponent<RectTransform>();
        if (entryRect != null)
        {
            entryRect.localScale = Vector3.one;
            entryRect.anchorMin = new Vector2(0, 1); // Anchor top-left
            entryRect.anchorMax = new Vector2(1, 1); // Stretch width
            entryRect.pivot = new Vector2(0.5f, 1); // Pivot top-center
            entryRect.sizeDelta = new Vector2(0, itemHeight); // Width fit parent, height cố định
            entryRect.anchoredPosition = new Vector2(0, yOffset); // Đặt vị trí dọc
            yOffset -= (itemHeight + spacing); // Di chuyển xuống cho item tiếp theo
        }

        InventoryItemEntry itemEntry = entry.GetComponent<InventoryItemEntry>();
        if (itemEntry == null)
        {
            return;
        }

        itemEntry.Setup(itemId, quantity, this);
        TextMeshProUGUI text = itemEntry.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
        }
    }
    public void SetSelectedEntry(InventoryItemEntry entry)
    {
        if (selectedEntry != null && selectedEntry != entry)
        {
            selectedEntry.SetSelected(false);
        }

        selectedEntry = entry;

        if (selectedEntry != null)
        {
            selectedEntry.SetSelected(true);
        }
    }

    public void ShowPreview(string itemId)
    {
        ClearPreview();
        GameObject prefab = LoadItemPrefab(itemId); 
        if (prefab != null && previewSpawnPoint != null)
        {
            currentPreviewObject = Instantiate(prefab, previewSpawnPoint.position, previewSpawnPoint.rotation);
            currentPreviewObject.transform.SetParent(previewSpawnPoint);
        }
    }

    private void ClearPreview()
    {
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
            currentPreviewObject = null;
        }
    }
    private GameObject LoadItemPrefab(string itemId) 
    {
        return Resources.Load<GameObject>($"ItemPrefabs/{itemId}");
    }
}