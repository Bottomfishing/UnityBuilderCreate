using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("背包UI设置")]
    public Transform slotContainer;
    public GameObject slotPrefab;

    [Header("资源设置")]
    public Sprite coinIcon;
    public Sprite diamondIcon;
    public Sprite energyIcon;

    private List<InventorySlot> _slots = new List<InventorySlot>();

    private void Awake()
    {
        InitializeSlots();
    }

    private void Start()
    {
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnResourceChanged += RefreshInventory;
        }
        RefreshInventory();
    }

    private void OnDestroy()
    {
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnResourceChanged -= RefreshInventory;
        }
    }

    private void InitializeSlots()
    {
        if (slotContainer == null || slotPrefab == null)
        {
            Debug.LogError("SlotContainer 或 SlotPrefab 未设置！");
            return;
        }

        foreach (Transform child in slotContainer)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
            {
                _slots.Add(slot);
            }
        }

        if (_slots.Count == 0 && InventoryManager.instance != null)
        {
            for (int i = 0; i < InventoryManager.instance.inventorySize; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotContainer);
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    _slots.Add(slot);
                }
            }
        }
    }

    public void RefreshInventory()
    {
        if (ResourceManager.instance == null)
        {
            return;
        }

        if (_slots.Count > 0)
        {
            InventoryItem coinItem = new InventoryItem(
                "resource_coin",
                "金币",
                coinIcon,
                ItemType.Material,
                ItemRarity.Normal,
                999999,
                ResourceManager.instance.GetCoins(),
                "游戏货币"
            );
            _slots[0].SetItem(coinItem);
        }

        if (_slots.Count > 1)
        {
            InventoryItem diamondItem = new InventoryItem(
                "resource_diamond",
                "钻石",
                diamondIcon,
                ItemType.Material,
                ItemRarity.Excellent,
                999999,
                ResourceManager.instance.GetDiamonds(),
                "珍贵货币"
            );
            _slots[1].SetItem(diamondItem);
        }

        if (_slots.Count > 2)
        {
            InventoryItem energyItem = new InventoryItem(
                "resource_energy",
                "体力",
                energyIcon,
                ItemType.Consumable,
                ItemRarity.Fine,
                999999,
                ResourceManager.instance.GetEnergy(),
                "用于挑战关卡"
            );
            _slots[2].SetItem(energyItem);
        }

        if (InventoryManager.instance == null) return;

        for (int i = 3; i < _slots.Count; i++)
        {
            int inventoryIndex = i - 3;
            if (inventoryIndex < InventoryManager.instance.items.Count)
            {
                _slots[i].SetItem(InventoryManager.instance.items[inventoryIndex]);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }
    }
}
