using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

        foreach (ShopItem item in shopItems)
        {
            if (item == null) continue;

            GameObject itemObj = Instantiate(shopItemPrefab, contentContainer);
            ShopItemDisplay display = itemObj.GetComponent<ShopItemDisplay>();

            if (display != null)
            {
                display.Setup(item);
                currentDisplays.Add(display);
            }
        }
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
