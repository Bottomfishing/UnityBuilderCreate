using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("背包设置")]
    public int inventorySize = 30;
    public List<InventoryItem> items = new List<InventoryItem>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadInventory();
    }

    public bool AddItem(InventoryItem item)
    {
        if (item == null) return false;

        foreach (InventoryItem existingItem in items)
        {
            if (existingItem.CanStackWith(item))
            {
                int spaceLeft = existingItem.maxStackSize - existingItem.currentStackSize;
                int toAdd = Mathf.Min(spaceLeft, item.currentStackSize);
                existingItem.currentStackSize += toAdd;
                item.currentStackSize -= toAdd;

                if (item.currentStackSize <= 0)
                {
                    SaveInventory();
                    return true;
                }
            }
        }

        while (item.currentStackSize > 0 && items.Count < inventorySize)
        {
            int toAdd = Mathf.Min(item.maxStackSize, item.currentStackSize);
            InventoryItem newItem = new InventoryItem(
                item.itemId,
                item.itemName,
                item.itemIcon,
                item.itemType,
                item.rarity,
                item.maxStackSize,
                toAdd,
                item.description
            );
            items.Add(newItem);
            item.currentStackSize -= toAdd;
        }

        SaveInventory();
        return item.currentStackSize <= 0;
    }

    public bool RemoveItem(string itemId, int count = 1)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemId == itemId)
            {
                int toRemove = Mathf.Min(count, items[i].currentStackSize);
                items[i].currentStackSize -= toRemove;
                count -= toRemove;

                if (items[i].currentStackSize <= 0)
                {
                    items.RemoveAt(i);
                }

                if (count <= 0)
                {
                    SaveInventory();
                    return true;
                }
            }
        }

        SaveInventory();
        return false;
    }

    public int GetItemCount(string itemId)
    {
        int count = 0;
        foreach (InventoryItem item in items)
        {
            if (item.itemId == itemId)
            {
                count += item.currentStackSize;
            }
        }
        return count;
    }

    public bool HasItem(string itemId, int count = 1)
    {
        return GetItemCount(itemId) >= count;
    }

    public void ClearInventory()
    {
        items.Clear();
        SaveInventory();
    }

    private void SaveInventory()
    {
        string json = JsonUtility.ToJson(new InventorySaveData { items = items });
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("InventoryData"))
        {
            string json = PlayerPrefs.GetString("InventoryData");
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);
            if (saveData != null && saveData.items != null)
            {
                items = saveData.items;
            }
        }
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItem> items;
}
