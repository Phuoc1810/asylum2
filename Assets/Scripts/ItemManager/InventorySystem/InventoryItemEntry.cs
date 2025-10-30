using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI itemNameText;

    [Header("Text Settings")]
    [SerializeField] private float normalFontSize = 20f;
    [SerializeField] private float selectedFontSize = 28f;
    [SerializeField] private float normalAlpha = 0.8f;
    [SerializeField] private float selectedAlpha = 1f;
    [SerializeField] private FontStyles normalFontStyle = FontStyles.Normal;
    [SerializeField] private FontStyles selectedFontStyle = FontStyles.Bold;

    private string itemId;
    private InventoryUI inventoryUI;
    private bool isSelected = false;

    private void Awake()
    {
        

        SetNormalState();
    }

    public void Setup(string id, int quantity, InventoryUI ui)
    {
        itemId = id;
        inventoryUI = ui;

        if (itemNameText != null)
        {
            itemNameText.text = GetDisplayName(itemId);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryUI != null)
        {
            inventoryUI.ShowPreview(itemId);
            inventoryUI.SetSelectedEntry(this);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Không làm gì cả, giữ nguyên hover đã chọn
    }
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selected)
        {
            SetSelectedState();
        }
        else
        {
            SetNormalState();
        }
    }
    private void SetSelectedState()
    {
        if (itemNameText != null)
        {
            itemNameText.fontSize = selectedFontSize;
            itemNameText.fontStyle = selectedFontStyle;

            Color currentColor = itemNameText.color;
            itemNameText.color = new Color(currentColor.r, currentColor.g, currentColor.b, selectedAlpha);
        }
    }

    private void SetNormalState()
    {
        if (itemNameText != null)
        {
            itemNameText.fontSize = normalFontSize;
            itemNameText.fontStyle = normalFontStyle;

            Color currentColor = itemNameText.color;
            itemNameText.color = new Color(currentColor.r, currentColor.g, currentColor.b, normalAlpha);
        }
    }

    private string GetDisplayName(string id)
    {
        switch (id)
        {
            case "screwdriver": return "Tua vít";//1
            case "fuse": return "Cầu chì";//2
            case "key_maintenance": return "Chìa khóa phòng bảo trì";//3
            case "flashlight": return "Đèn pin";//4
            case "director_key": return "Chìa khóa phòng viện trưởng";//4
            case "broading_key": return "Chìa khóa phòng nội trú 1";//5
            case "endgame_key": return "Chìa khóa cuối cùng";//6
            case "Electric_Room_key": return "Chìa khóa phòng điện";//7
            case "Key_Morgue": return "Chìa khóa nhà xác";//8
            case "strorage_key":return "Chìa khóa kho hàng";//9
            case "wc_key": return "Chìa khóa nhà vệ sinh";//10
            case "xq_key": return "Chìa khóa phòng X-quang";//11
            case "Document_Room_Key": return "Chìa khóa phòng hồ sơ bệnh án";//12
            case "file_key": return "Chìa khóa tủ hồ sơ";//13
            case "battery": return "Pin dự phòng";//14
            case "bolt_cutter": return "Kìm cộng lực";//15
            case "crowbar": return "Xà beng";//16   
            case "hammer": return "Búa tạ";//17
            case "scissors": return "Kéo cắt dây";//18


            default: return id;
        }
    }

    private void OnValidate()
    {
        if (itemNameText == null)
        {
            itemNameText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}