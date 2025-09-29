using UnityEngine;
using TMPro;

public class UIinven : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Text đích để liệt kê item")]
    public TMP_Text listText;          // kéo Text (TMP)vào đây
    [SerializeField] int maxLines = 50;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddLine(string line)
    {
        if (listText == null || string.IsNullOrEmpty(line)) return;

        if (string.IsNullOrEmpty(listText.text)) listText.text = line;
        else listText.text += "\n" + line;

        // Optional: giới hạn số dòng
        var lines = listText.text.Split('\n');
        if (lines.Length > maxLines)
            listText.text = string.Join("\n", lines, lines.Length - maxLines, maxLines);
    }
}
