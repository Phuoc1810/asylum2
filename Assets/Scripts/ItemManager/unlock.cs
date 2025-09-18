using UnityEngine;

public class unlock : MonoBehaviour
{
   public bool hasScrewdriver;
    public Door door;
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
    }
}
