using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;      // kéo inventory system vào
    [SerializeField] private Transform slotsRoot;      // kéo Inventoryslot vào
    [SerializeField] private GameObject slotUIPrefab;  // kéo prefab SlotUI vào

    private List<slotUI> spawned = new List<slotUI>();

    void Start()
    {
        // Spawn ?úng b?ng s? slot c?a Inventory
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            var go = Instantiate(slotUIPrefab, slotsRoot);
            spawned.Add(go.GetComponent<slotUI>());
        }
        inventory.OnChanged += Refresh;
        Refresh();
    }

    void OnDestroy() { if (inventory != null) inventory.OnChanged -= Refresh; }

    public void Refresh()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            var s = inventory.slots[i];
            if (s == null || s.IsEmpty) spawned[i].Set(null, 0);
            else spawned[i].Set(s.item ? s.item.icon : null, s.quantity);
        }
    }
}
