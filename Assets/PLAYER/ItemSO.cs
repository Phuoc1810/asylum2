using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item", fileName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public bool stackable = true;
    public int maxStack = 99;
}
