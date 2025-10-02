using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class InventoryService : MonoBehaviour
{
    public static InventoryService Instance { get; private set; }

    [SerializeField] private bool dontDestroyOnLoad = true;

    [SerializeField] private readonly Dictionary<string, int> _items = new Dictionary<string, int>(StringComparer.Ordinal);

    private event Action OnChanged;

    public event Action Changed
    {
        add { OnChanged += value; }
        remove { OnChanged -= value; }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);

        }

    }
    public bool Contains(string itemID) => _items.TryGetValue(itemID, out var count) && count > 0;
    public int GetQuantity(string intemID) => _items.TryGetValue(intemID, out var count) ? count : 0;

    public void Add(string itemID, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemID) || amount <= 0) return;
        _items[itemID] = GetQuantity(itemID) + amount;
        OnChanged?.Invoke();
    }

    public bool Remove(string itemID, int amount = 1)
    {
        if (!Contains(itemID) || amount <= 0) return false;
        var left = _items[itemID] - amount;
        if (left > 0) _items[itemID] = left;
        else _items.Remove(itemID);

        OnChanged?.Invoke();
        return true;
    }

    public IReadOnlyDictionary<string, int> SnapShot() => _items;

    public void Clear()
    {
        _items.Clear();
        OnChanged?.Invoke();
    }
}

