using UnityEngine;
using UnityEngine.InputSystem;

public class unlock : MonoBehaviour
{
    public bool hasScrewdriver;
    public bool haskeytool;
    public bool haskeyblackkey;
    public Door door;
    public Door doortool;
    public Door blackdoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        hasScrewdriver = InventoryService.Instance != null && InventoryService.Instance.Contains("Document_Room_Key"); 
        if (hasScrewdriver)
        {
            door.locks = false;
        }
        haskeytool = InventoryService.Instance != null && InventoryService.Instance.Contains("Key_Morgue"); 
        if (haskeytool)
        {
            doortool.locks = false;
        }
        haskeyblackkey = InventoryManager.instance.HasItem(Interactable.InteracType.blackkey);
        if (haskeyblackkey)
        {
            blackdoor.locks = false;
        }
    }
}
