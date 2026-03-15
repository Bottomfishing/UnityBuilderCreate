using UnityEngine;

public enum ItemType
{
    Consumable,
    Material,
    Equipment,
    Special
}

public enum ItemRarity
{
    Normal,
    Fine,
    Excellent,
    Perfect,
    Ancient,
    Divine
}

[System.Serializable]
public class InventoryItem
{
    public string itemId;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public ItemRarity rarity;
    public int maxStackSize = 9999;
    public int currentStackSize = 1;
    public string description;

    public InventoryItem(string id, string name, Sprite icon, ItemType type, ItemRarity itemRarity = ItemRarity.Normal, int maxStack = 9999, int count = 1, string desc = "")
    {
        itemId = id;
        itemName = name;
        itemIcon = icon;
        itemType = type;
        rarity = itemRarity;
        maxStackSize = maxStack;
        currentStackSize = count;
        description = desc;
    }

    public bool CanStackWith(InventoryItem other)
    {
        return itemId == other.itemId && currentStackSize < maxStackSize;
    }
}
