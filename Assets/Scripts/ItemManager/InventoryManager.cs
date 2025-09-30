using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Inventory Data")]
    public List<GameObject> items = new List<GameObject>();

    [Header("UI Toggle")]
    [Tooltip("Gán Panel/Canvas gốc của Inventory (object cần bật/tắt)")]
    public GameObject inventoryUIRoot;
    [Tooltip("Phím mở/đóng Inventory")]
    public KeyCode toggleKey = KeyCode.Tab;
    [Tooltip("Tạm dừng game khi mở Inventory")]
    public bool pauseOnOpen = false;
    [Tooltip("Mở Inventory sẽ hiện chuột, đóng thì khóa chuột (hữu ích cho FPS)")]
    public bool unlockCursorOnOpen = true;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // đảm bảo UI khởi tạo đúng trạng thái
        SetOpen(false, applySideEffects: true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    //================= TOGGLE UI =================
    public void Toggle()
    {
        SetOpen(!IsOpen, applySideEffects: true);
    }

    public void Open() => SetOpen(true, applySideEffects: true);
    public void Close() => SetOpen(false, applySideEffects: true);

    private void SetOpen(bool open, bool applySideEffects)
    {
        IsOpen = open;
        if (inventoryUIRoot) inventoryUIRoot.SetActive(open);

        if (!applySideEffects) return;

        // Pause/Resume
        if (pauseOnOpen)
            Time.timeScale = open ? 0f : 1f;

        // Cursor lock/visibility (thường dùng trong game FPS)
        if (unlockCursorOnOpen)
        {
            Cursor.visible = open;
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    //================= ITEMS API (giữ nguyên + thêm Remove) =================
    public void AddItems(GameObject item)
    {
        if (item != null) items.Add(item);
    }

    public bool HasItem(Interactable.InteracType itemType)
    {
        foreach (GameObject item in items)
        {
            var interactable = item.GetComponent<Interactable>();
            if (interactable != null && interactable.Type == itemType)
                return true;
        }
        return false;
    }

    public void RemoveItem(Interactable.InteracType itemType)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            var interactable = items[i].GetComponent<Interactable>();
            if (interactable != null && interactable.Type == itemType)
                items.RemoveAt(i);
        }
    }
}
