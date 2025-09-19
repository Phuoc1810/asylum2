using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotePanelController : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        [Header("N�t ? c?t tr�i")]
        public Button button;                 // <-- k�o Button "abc/abc1/abc2" v�o ?�y
        [Header("D? li?u hi?n th?")]
        public string title;                  // kh�ng b?t bu?c, n?u c?n
        [TextArea(2, 8)] public string text;  // n?i dung m� t? hi?n th? b�n ph?i
        public Sprite image;                  // h�nh preview (vd t? gi?y)

        [Header("Th??ng item khi click (tu? ch?n)")]
        public ItemSO rewardItem;             // k�o ScriptableObject item (vd: key, note)
        public int rewardAmount = 1;
        public bool giveOnSelect = false;     // b?t l�n n?u mu?n click l� c?ng ??
    }

    [Header("UI ?�ch ? panel b�n ph?i")]
    public Image previewImage;                // ?nh to b�n ph?i (vd t? gi?y)
    public TextMeshProUGUI previewText;       // text m� t? b�n ph?i

    [Header("Danh s�ch m?c (abc, abc1, abc2)")]
    public Entry[] entries;

    int current = -1;

    void Awake()
    {
        // G?n h�nh vi click cho t?ng button ? c?t tr�i
        for (int i = 0; i < entries.Length; i++)
        {
            int idx = i; // capture
            if (entries[idx].button != null)
                entries[idx].button.onClick.AddListener(() => Select(idx));
        }

        // M?c ??nh m? m?c ??u ti�n (n?u c�)
        if (entries.Length > 0) Select(0);
    }

    public void Select(int index)
    {
        if (index < 0 || index >= entries.Length) return;
        current = index;

        var e = entries[index];

        // Hi?n th? text/h�nh
        if (previewText) previewText.text = e.text ?? "";
        if (previewImage)
        {
            previewImage.enabled = (e.image != null);
            previewImage.sprite = e.image;
        }

        // (Tu? ch?n) th??ng item v�o inventory khi click
        if (e.giveOnSelect && e.rewardItem != null)
        {
            // H�m AddItem c?a b?n: AddItem(ItemSO item, int amount = 1, bool forceNewStack = false)
            Inventory.I?.AddItem(e.rewardItem, e.rewardAmount, true);
        }
    }
}
