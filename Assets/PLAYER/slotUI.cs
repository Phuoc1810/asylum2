using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class slotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI qty;

    void Awake()
    {
        if (!icon)
        {
            var t = transform.Find("Icon");
            if (t) icon = t.GetComponent<Image>();
            if (!icon) icon = GetComponentInChildren<Image>(true);
        }
        if (!qty)
        {
            var t = transform.Find("Qty");
            if (t) qty = t.GetComponent<TextMeshProUGUI>();
            if (!qty) qty = GetComponentInChildren<TextMeshProUGUI>(true);
        }
    }

    public void Set(Sprite s, int amount)
    {
        if (icon)
        {
            icon.sprite = s;
            icon.enabled = s != null;
        }
        if (qty)
        {
            if (amount > 1) { qty.text = amount.ToString(); qty.enabled = true; }
            else { qty.text = ""; qty.enabled = false; }
        }
    }
}
