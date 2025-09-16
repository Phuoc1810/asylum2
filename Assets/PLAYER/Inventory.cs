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
        item != null && other != null &&
        item == other && item.stackable && quantity < item.maxStack;
}

public class Inventory : MonoBehaviour
{
    public static Inventory I { get; private set; }

    [SerializeField] int slotCount = 20;
    public List<InventorySlot> slots = new();
    public event Action OnChanged;

    public int SlotCount => slotCount;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        slots.Clear();
        for (int i = 0; i < slotCount; i++)
            slots.Add(new InventorySlot());
    }

    
    public bool AddItem(ItemSO item, int amount = 1, bool forceNewStack = false)
    {
        if (item == null || amount <= 0) return false;

        // 1) Neu cho phép stack & không ép tao stack moi -> ghep vào slot dang co
        if (item.stackable && !forceNewStack)
        {
            foreach (var s in slots)
            {
                if (s.CanStack(item))
                {
                    int add = Mathf.Min(amount, item.maxStack - s.quantity);
                    s.quantity += add;
                    amount -= add;
                    if (amount <= 0) { OnChanged?.Invoke(); return true; }
                }
            }
        }

        // 2) nhet vào Ô TRONG UU TIEN (=> món 2 so vào slot 2 neu slot 1 da co mon 1)
        foreach (var s in slots)
        {
            if (s.IsEmpty)
            {
                s.item = item;
                s.quantity = Mathf.Min(amount, item.stackable ? item.maxStack : 1);
                amount -= s.quantity;
                OnChanged?.Invoke();
                return true;
            }
        }
        OnChanged?.Invoke();
        return amount == 0;
    }
}
