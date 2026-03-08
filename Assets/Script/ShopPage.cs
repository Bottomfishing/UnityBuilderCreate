using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ShopPage : MonoBehaviour
{
    [Header("商店商品配置")]
    public ShopItem[] shopItems;

    [Header("UI组件")]
    public Transform contentContainer;
    public GameObject shopItemPrefab;

    private List<ShopItemDisplay> currentDisplays = new List<ShopItemDisplay>();

    private void Start()
    {
        RefreshShop();
    }

    public void RefreshShop()
    {
        ClearCurrentDisplays();

        if (shopItems == null || shopItemPrefab == null || contentContainer == null)
        {
            return;
        }

        List<ShopItem> sortedItems = SortShopItems(shopItems);

        foreach (ShopItem item in sortedItems)
        {
            if (item == null) continue;

            GameObject itemObj = Instantiate(shopItemPrefab, contentContainer);
            ShopItemDisplay display = itemObj.GetComponent<ShopItemDisplay>();

            if (display != null)
            {
                display.Setup(item, this);
                currentDisplays.Add(display);
            }
        }
    }

    private List<ShopItem> SortShopItems(ShopItem[] items)
    {
        List<ShopItem> notPurchased = new List<ShopItem>();
        List<ShopItem> purchased = new List<ShopItem>();

        foreach (ShopItem item in items)
        {
            if (item == null) continue;

            if (IsItemPurchased(item))
            {
                purchased.Add(item);
            }
            else
            {
                notPurchased.Add(item);
            }
        }

        notPurchased.AddRange(purchased);
        return notPurchased;
    }

    private bool IsItemPurchased(ShopItem item)
    {
        if (item == null) return false;

        if (item.type == ShopItemType.TowerUnlock)
        {
            if (TowerUnlockManager.instance != null && !string.IsNullOrEmpty(item.towerNameToUnlock))
            {
                return TowerUnlockManager.instance.IsTowerUnlocked(item.towerNameToUnlock);
            }
        }
        else if (!item.canRepeatPurchase)
        {
            return false;
        }

        return false;
    }

    private void ClearCurrentDisplays()
    {
        foreach (ShopItemDisplay display in currentDisplays)
        {
            if (display != null && display.gameObject != null)
            {
                Destroy(display.gameObject);
            }
        }
        currentDisplays.Clear();
    }
}
