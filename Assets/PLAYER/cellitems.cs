using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellItemUI : MonoBehaviour
{
    [Header("Refs on this cell")]
    [SerializeField] Image iconImage;         // ảnh chính trong cell
    [SerializeField] TMP_Text nameText;       // (tuỳ chọn) tên dưới icon
    [SerializeField] TMP_Text amountText;     // (tuỳ chọn) số lượng ở góc

    /// <summary>Đổ dữ liệu từ ItemSO (có sprite icon)</summary>
    public void SetItem(ItemSO item, int amount = 1, bool showName = false)
    {
        if (!item) { SetEmpty(); return; }

        // icon
        if (iconImage)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = item.icon != null;
            iconImage.preserveAspect = true;
        }

        // tên (nếu muốn)
        if (nameText)
        {
            nameText.gameObject.SetActive(showName);
            if (showName)
                nameText.text = string.IsNullOrEmpty(item.itemName) ? item.name : item.itemName;
        }

        // số lượng
        if (amountText)
        {
            bool showAmount = amount > 1;
            amountText.gameObject.SetActive(showAmount);
            if (showAmount) amountText.text = amount.ToString();
        }
    }

    /// <summary>Xoá dữ liệu trong ô</summary>
    public void SetEmpty()
    {
        if (iconImage)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
        if (nameText) nameText.gameObject.SetActive(false);
        if (amountText) amountText.gameObject.SetActive(false);
    }
}
