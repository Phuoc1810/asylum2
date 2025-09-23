using UnityEngine;

public class unlock : MonoBehaviour
{
   public bool hasScrewdriver;
    public bool haskeytool;
    public Door door;
    public Door doortool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.keymorgue);
        if(hasScrewdriver)
        {
            door.locks=false;
        }
        haskeytool = InventoryManager.instance.HasItem(Interactable.InteracType.keytools);
        if(haskeytool)
        {
            doortool.locks=false;
        }
    }
}
