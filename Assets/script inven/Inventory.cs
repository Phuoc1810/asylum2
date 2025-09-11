using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemSO item;
    public int quantity;
    public bool IsEmpty => item == null || quantity <= 0;
    public bool CanStack(ItemSO other) =>
        item != null && other != null && item == other && item.stackable && quantity < item.maxStack;
}

public class Inventory : MonoBehaviour
{
    public static Inventory I { get; private set; }

    [SerializeField] private int slotCount = 20;  // s? ô b?n mu?n spawn
    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnChanged;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        slots.Clear();
        for (int i = 0; i < slotCount; i++) slots.Add(new InventorySlot());
    }

    public bool AddItem(ItemSO item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        if (item.stackable)
        {
            foreach (var s in slots)
            {
                if (s.CanStack(item))
                {
                    int canAdd = Mathf.Min(item.maxStack - s.quantity, amount);
                    s.quantity += canAdd;
                    amount -= canAdd;
                    if (amount <= 0) { OnChanged?.Invoke(); return true; }
                }
            }
        }

        foreach (var s in slots)
        {
            if (s.IsEmpty)
            {
                s.item = item;
                s.quantity = Mathf.Min(item.stackable ? item.maxStack : 1, amount);
                amount -= s.quantity;
                if (amount <= 0) { OnChanged?.Invoke(); return true; }
            }
        }
        OnChanged?.Invoke();
        return false; // h?t ch?
    }
}
