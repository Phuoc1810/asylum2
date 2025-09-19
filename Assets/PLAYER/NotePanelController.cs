using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotePanelController : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        [Header("Nút ? c?t trái")]
        public Button button;                 // <-- kéo Button "abc/abc1/abc2" vào ?ây
        [Header("D? li?u hi?n th?")]
        public string title;                  // không b?t bu?c, n?u c?n
        [TextArea(2, 8)] public string text;  // n?i dung mô t? hi?n th? bên ph?i
        public Sprite image;                  // hình preview (vd t? gi?y)

        [Header("Th??ng item khi click (tu? ch?n)")]
        public ItemSO rewardItem;             // kéo ScriptableObject item (vd: key, note)
        public int rewardAmount = 1;
        public bool giveOnSelect = false;     // b?t lên n?u mu?n click là c?ng ??
    }

    [Header("UI ?ích ? panel bên ph?i")]
    public Image previewImage;                // ?nh to bên ph?i (vd t? gi?y)
    public TextMeshProUGUI previewText;       // text mô t? bên ph?i

    [Header("Danh sách m?c (abc, abc1, abc2)")]
    public Entry[] entries;

    int current = -1;

    void Awake()
    {
        // G?n hành vi click cho t?ng button ? c?t trái
        for (int i = 0; i < entries.Length; i++)
        {
            int idx = i; // capture
            if (entries[idx].button != null)
                entries[idx].button.onClick.AddListener(() => Select(idx));
        }

        // M?c ??nh m? m?c ??u tiên (n?u có)
        if (entries.Length > 0) Select(0);
    }

    public void Select(int index)
    {
        if (index < 0 || index >= entries.Length) return;
        current = index;

        var e = entries[index];

        // Hi?n th? text/hình
        if (previewText) previewText.text = e.text ?? "";
        if (previewImage)
        {
            previewImage.enabled = (e.image != null);
            previewImage.sprite = e.image;
        }

        // (Tu? ch?n) th??ng item vào inventory khi click
        if (e.giveOnSelect && e.rewardItem != null)
        {
            // Hàm AddItem c?a b?n: AddItem(ItemSO item, int amount = 1, bool forceNewStack = false)
            Inventory.I?.AddItem(e.rewardItem, e.rewardAmount, true);
        }
    }
}
