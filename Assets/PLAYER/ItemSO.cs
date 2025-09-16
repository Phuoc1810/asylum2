using UnityEngine;

public enum ItemType { Key }

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item", order = 0)]
public class ItemSO : ScriptableObject
{
    public string rust_key_01;
    public string Key;
    public ItemType type = ItemType.Key;
    public bool stackable = false;
    public Sprite icon;
    internal int maxStack;
}
