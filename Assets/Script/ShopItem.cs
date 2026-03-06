using UnityEngine;

public enum ShopItemType
{
    Coins,
    Diamonds,
    Energy,
    TowerUnlock
}

[System.Serializable]
public class ShopItem
{
    [Header("基本信息")]
    public string itemName;
    public string description;
    public Sprite icon;
    public ShopItemType type;

    [Header("标签设置")]
    public bool showHotTag = false;
    public bool showLimitedTag = false;

    [Header("购买设置")]
    public bool canRepeatPurchase = true;

    [Header("价格")]
    public int price;
    public bool useDiamonds;

    [Header("炮塔解锁（仅炮塔类型需要）")]
    public string towerNameToUnlock;

    [Header("资源数量（仅资源类型需要）")]
    public int amount;
}
