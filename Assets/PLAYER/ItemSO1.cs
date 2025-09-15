using UnityEngine;

public enum ItemType { Capsule }

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item", order = 0)]
public class ItemSO : ScriptableObject
{
    public string rust_key_01;
    public string Key;
    public ItemType type = ItemType.Capsule;
    public bool stackable = false;
    public Sprite icon;
    internal int maxStack;
}
