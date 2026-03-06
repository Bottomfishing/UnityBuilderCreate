using UnityEngine;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;
    public Text nameText;
    public Text descriptionText;
    public Text priceText;
    public Image currencyIcon;
    public Button buyButton;
    public Text buyButtonText;
    
    [Header("标签UI")]
    public GameObject hotTag;
    public GameObject limitedTag;
    
    [Header("货币图标")]
    public Sprite coinIcon;
    public Sprite diamondIcon;

    private ShopItem item;
    private bool isPurchased = false;

    public void Setup(ShopItem shopItem)
    {
        item = shopItem;
        CheckIfPurchased();
        UpdateUI();
    }

    private void CheckIfPurchased()
    {
        if (item.type == ShopItemType.TowerUnlock)
        {
            if (TowerUnlockManager.instance != null && !string.IsNullOrEmpty(item.towerNameToUnlock))
            {
                isPurchased = TowerUnlockManager.instance.IsTowerUnlocked(item.towerNameToUnlock);
            }
        }
        else
        {
            isPurchased = false;
        }
    }

    private void UpdateUI()
    {
        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
        }

        if (nameText != null)
        {
            nameText.text = item.itemName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = item.description;
        }

        if (priceText != null)
        {
            priceText.text = item.price.ToString();
        }

        if (currencyIcon != null)
        {
            if (item.useDiamonds)
            {
                currencyIcon.sprite = diamondIcon;
            }
            else
            {
                currencyIcon.sprite = coinIcon;
            }
            currencyIcon.gameObject.SetActive(true);
        }

        if (hotTag != null)
        {
            hotTag.SetActive(item.showHotTag);
        }

        if (limitedTag != null)
        {
            limitedTag.SetActive(item.showLimitedTag);
        }

        UpdateBuyButton();
    }

    private void UpdateBuyButton()
    {
        if (buyButton == null) return;

        buyButton.onClick.RemoveAllListeners();

        if (isPurchased && !item.canRepeatPurchase)
        {
            buyButton.interactable = false;
            if (buyButtonText != null)
            {
                buyButtonText.text = "已购买";
            }
        }
        else
        {
            buyButton.interactable = true;
            if (buyButtonText != null)
            {
                if (item.canRepeatPurchase)
                {
                    buyButtonText.text = "兑换";
                }
                else
                {
                    buyButtonText.text = "购买";
                }
            }
            buyButton.onClick.AddListener(OnBuyClick);
        }
    }

    private void OnBuyClick()
    {
        if (isPurchased && !item.canRepeatPurchase) return;

        if (TrySpendMoney())
        {
            GiveReward();
            
            if (!item.canRepeatPurchase)
            {
                MarkAsPurchased();
                isPurchased = true;
            }
            
            UpdateBuyButton();
        }
    }

    private bool TrySpendMoney()
    {
        if (ResourceManager.instance == null) return true;

        if (item.useDiamonds)
        {
            return ResourceManager.instance.SpendDiamonds(item.price);
        }
        else
        {
            return ResourceManager.instance.SpendCoins(item.price);
        }
    }

    private void GiveReward()
    {
        switch (item.type)
        {
            case ShopItemType.Coins:
                if (ResourceManager.instance != null)
                {
                    ResourceManager.instance.AddCoins(item.amount);
                }
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddMoney(item.amount);
                }
                break;

            case ShopItemType.Diamonds:
                if (ResourceManager.instance != null)
                {
                    ResourceManager.instance.AddDiamonds(item.amount);
                }
                break;

            case ShopItemType.Energy:
                if (ResourceManager.instance != null)
                {
                    ResourceManager.instance.AddEnergy(item.amount);
                }
                break;

            case ShopItemType.TowerUnlock:
                if (TowerUnlockManager.instance != null && !string.IsNullOrEmpty(item.towerNameToUnlock))
                {
                    TowerUnlockManager.instance.UnlockTower(item.towerNameToUnlock);
                }
                break;
        }
    }

    private void MarkAsPurchased()
    {
    }
}
