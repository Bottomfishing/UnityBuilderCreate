using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerBook : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject towerBookUI;
    public Button closeButton;
    public Transform contentContainer;
    public GameObject towerItemPrefab;
    public GraphicRaycaster graphicRaycaster;
    
    [Header("炮塔数据")]
    public TowerDataForBook[] towers;
    
    private bool isInitialized = false;

    private void Awake()
    {
        DoInit();
    }
    
    private void DoInit()
    {
        if (isInitialized) return;
        isInitialized = true;
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseTowerBook);
        }
        
        GenerateTowerItems();
        
        if (towerBookUI != null)
            towerBookUI.SetActive(false);
    }
    
    private void GenerateTowerItems()
    {
        if (contentContainer == null || towerItemPrefab == null || towers == null)
        {
            return;
        }
        
        foreach (Transform child in contentContainer)
        {
            if (child != contentContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (TowerDataForBook tower in towers)
        {
            if (tower != null)
            {
                GameObject itemObj = Instantiate(towerItemPrefab, contentContainer);
                SetupTowerItem(itemObj, tower);
            }
        }
    }
    
    private void SetupTowerItem(GameObject itemObj, TowerDataForBook tower)
    {
        if (itemObj == null || tower == null)
        {
            return;
        }
        
        Text nameText = itemObj.transform.Find("NameText")?.GetComponent<Text>();
        if (nameText != null)
        {
            nameText.text = tower.towerName;
        }
        
        Image iconImage = itemObj.transform.Find("IconImage")?.GetComponent<Image>();
        if (iconImage != null && tower.towerIcon != null)
        {
            iconImage.sprite = tower.towerIcon;
        }
        
        Text descText = itemObj.transform.Find("DescText")?.GetComponent<Text>();
        if (descText != null)
        {
            descText.text = tower.description;
        }
        
        Text costText = itemObj.transform.Find("CostText")?.GetComponent<Text>();
        if (costText != null)
        {
            if (tower.source == TowerSource.Buildable)
            {
                costText.text = $"费用: {tower.buildCost}";
                costText.color = Color.white;
            }
            else
            {
                if (!string.IsNullOrEmpty(tower.upgradeFromTower))
                {
                    costText.text = $"由 {tower.upgradeFromTower} 升级";
                }
                else
                {
                    costText.text = "仅可升级获得";
                }
                costText.color = Color.yellow;
            }
        }
        
        Text rangeText = itemObj.transform.Find("RangeText")?.GetComponent<Text>();
        if (rangeText != null)
        {
            rangeText.text = $"范围: {tower.attackRange}";
        }
        
        Text damageText = itemObj.transform.Find("DamageText")?.GetComponent<Text>();
        if (damageText != null)
        {
            damageText.text = $"伤害: {tower.attackDamage}";
        }
        
        Text speedText = itemObj.transform.Find("SpeedText")?.GetComponent<Text>();
        if (speedText != null)
        {
            speedText.text = $"攻速: {tower.attackSpeed}";
        }
        
        Image backgroundImage = itemObj.GetComponent<Image>();
        if (backgroundImage != null)
        {
            if (tower.source == TowerSource.UpgradeOnly)
            {
                Color color = backgroundImage.color;
                color.r = 1f;
                color.g = 0.9f;
                color.b = 0.7f;
                backgroundImage.color = color;
            }
        }
    }
    
    public void OpenTowerBook()
    {
        if (!isInitialized) DoInit();
        
        if (towerBookUI != null)
        {
            towerBookUI.SetActive(true);
        }
    }
    
    public void CloseTowerBook()
    {
        if (towerBookUI != null)
        {
            towerBookUI.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (towerBookUI != null && towerBookUI.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIObject())
                {
                    CloseTowerBook();
                }
            }
        }
    }
    
    private bool IsPointerOverUIObject()
    {
        if (graphicRaycaster == null)
        {
            graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        }
        
        if (graphicRaycaster == null || towerBookUI == null)
        {
            return false;
        }
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.transform.IsChildOf(towerBookUI.transform))
            {
                return true;
            }
        }
        
        return false;
    }
}
