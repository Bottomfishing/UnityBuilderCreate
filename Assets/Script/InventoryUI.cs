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
        Debug.Log($"初始化槽位 - SlotContainer: {slotContainer != null}, SlotPrefab: {slotPrefab != null}");
        
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

        Debug.Log($"找到已有槽位数量: {_slots.Count}");

        if (_slots.Count == 0 && InventoryManager.instance != null)
        {
            Debug.Log($"开始生成槽位，总数: {InventoryManager.instance.inventorySize}");
            for (int i = 0; i < InventoryManager.instance.inventorySize; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotContainer);
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    _slots.Add(slot);
                }
            }
            Debug.Log($"生成后槽位总数: {_slots.Count}");
        }
    }

    public void RefreshInventory()
    {
        Debug.Log($"刷新背包 - Slot数量: {_slots.Count}, ResourceManager: {ResourceManager.instance != null}, CoinIcon: {coinIcon != null}");
        
        if (ResourceManager.instance == null)
        {
            Debug.LogWarning("ResourceManager 不存在！");
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
            Debug.Log($"设置金币: {ResourceManager.instance.GetCoins()}");
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
            Debug.Log($"设置钻石: {ResourceManager.instance.GetDiamonds()}");
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
            Debug.Log($"设置体力: {ResourceManager.instance.GetEnergy()}");
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
