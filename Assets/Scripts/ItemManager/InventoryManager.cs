using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Inventory Setting")]
    public List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddItems(GameObject item)
    {
        if (item != null)
        {
            items.Add(item);
        }
    }
    public bool HasItem(Interactable.InteracType itemType)
    {
        foreach(GameObject item in items)
        {
            Interactable interactable = item.GetComponent<Interactable>();
            if (interactable != null && interactable.Type == itemType)
            {
                return true;
            }
        }
        return false;
    }
    public void RemoveItem(Interactable.InteracType itemType)
    {
        for(int i = items.Count - 1; i >= 0; i--)
        {
            Interactable interactable = items[i].GetComponent<Interactable>();
            if (interactable != null && interactable.Type == itemType)
            {
                items.RemoveAt(i);
            }
        }
    }
}
