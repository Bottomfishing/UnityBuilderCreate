using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI组件")]
    public Image iconImage;
    public Text countText;
    public Image backgroundImage;
    public Image rarityBorderImage;

    [Header("视觉设置")]
    public Color slotNormalColor = Color.white;
    public Color selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    
    [Header("稀有度颜色")]
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color fineColor = new Color(0f, 0.8f, 0.3f, 1f);
    public Color excellentColor = new Color(0f, 0.5f, 1f, 1f);
    public Color perfectColor = new Color(0.9f, 0f, 0.9f, 1f);
    public Color ancientColor = new Color(1f, 0.6f, 0f, 1f);
    public Color divineColor = new Color(1f, 0.9f, 0f, 1f);

    private InventoryItem _currentItem;
    private bool _isSelected = false;

    public InventoryItem CurrentItem
    {
        get { return _currentItem; }
    }

    public bool IsEmpty
    {
        get { return _currentItem == null; }
    }

    private void Awake()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        ClearSlot();
    }

    public void SetItem(InventoryItem item)
    {
        _currentItem = item;

        if (item != null)
        {
            iconImage.sprite = item.itemIcon;
            iconImage.enabled = true;

            if (item.currentStackSize > 1)
            {
                countText.text = FormatNumber(item.currentStackSize);
                countText.enabled = true;
            }
            else
            {
                countText.enabled = false;
            }

            UpdateRarityBorder(item.rarity);
        }
        else
        {
            ClearSlot();
        }
    }

    private void UpdateRarityBorder(ItemRarity rarity)
    {
        if (rarityBorderImage != null)
        {
            Color borderColor = normalColor;
            switch (rarity)
            {
                case ItemRarity.Fine:
                    borderColor = fineColor;
                    break;
                case ItemRarity.Excellent:
                    borderColor = excellentColor;
                    break;
                case ItemRarity.Perfect:
                    borderColor = perfectColor;
                    break;
                case ItemRarity.Ancient:
                    borderColor = ancientColor;
                    break;
                case ItemRarity.Divine:
                    borderColor = divineColor;
                    break;
            }
            rarityBorderImage.color = borderColor;
            rarityBorderImage.enabled = true;
        }
    }

    private string FormatNumber(int number)
    {
        if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0.0") + "M";
        }
        else if (number >= 1000)
        {
            return (number / 1000f).ToString("0.0") + "K";
        }
        return number.ToString();
    }

    public void ClearSlot()
    {
        _currentItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        countText.text = "";
        countText.enabled = false;
        
        if (rarityBorderImage != null)
        {
            rarityBorderImage.enabled = false;
        }
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? selectedColor : slotNormalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            OnSlotClicked();
        }
    }

    private void OnSlotClicked()
    {
    }
}
