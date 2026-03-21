using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour
{
    public static bool IsUpgradeUIShowing { get; private set; }
    
    [Header("UI元素")]
    public Text towerNameText;
    public Button upgradeButton;
    public Text upgradeButtonText;
    public Button sellButton;
    public Text sellButtonText;
    public Button closeButton;
    public Button previewButton;
    
    [Header("UI位置设置")]
    public Vector2 uiPositionOffset = new Vector2(0, -50f);
    
    [Header("预览设置")]
    public Color originalTowerRangeColor = new Color(0, 0.8f, 1, 0.3f);
    public Color upgradedTowerRangeColor = new Color(0, 1, 0, 0.3f);
    public float previewLineWidth = 0.1f;
    
    private Tower currentTower;
    private TowerData currentTowerData;
    private Vector2Int currentGridPos;
    private GameObject previewTower;
    private LineRenderer originalTowerRange;
    private LineRenderer upgradedTowerRange;
    
    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideUI);
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeTower);
        }
        
        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellTower);
        }
        
        if (previewButton == null)
        {
            Transform previewBtnTrans = transform.Find("preview");
            if (previewBtnTrans != null)
            {
                previewButton = previewBtnTrans.GetComponent<Button>();
            }
        }
        
        if (previewButton != null)
        {
            previewButton.onClick.AddListener(TogglePreview);
        }
        
        IsUpgradeUIShowing = false;
        HideUI();
    }
    
    private void Update()
    {
        if (IsUpgradeUIShowing && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIElement(gameObject))
            {
                HideUI();
            }
        }
    }
    
    private bool IsPointerOverUIElement(GameObject uiElement)
    {
        if (uiElement == null)
        {
            return false;
        }
        
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return false;
        }
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            null,
            out localPoint
        );
        
        return rectTransform.rect.Contains(localPoint);
    }
    
    public void ShowUI(Tower tower, TowerData towerData, Vector2Int gridPos, Camera mainCamera)
    {
        currentTower = tower;
        currentTowerData = towerData;
        currentGridPos = gridPos;
        
        if (towerNameText != null)
        {
            towerNameText.text = towerData.towerName;
        }
        
        if (upgradeButton != null && upgradeButtonText != null)
        {
            if (towerData.upgradedTowerPrefab != null && towerData.upgradeCost > 0)
            {
                TowerData upgradedTowerData = FindUpgradedTowerData(towerData);
                bool isUpgradedTowerUnlocked = true;
                
                if (upgradedTowerData != null && TowerUnlockManager.instance != null)
                {
                    isUpgradedTowerUnlocked = TowerUnlockManager.instance.IsTowerUnlocked(upgradedTowerData.towerName);
                }
                
                upgradeButton.gameObject.SetActive(true);
                
                if (isUpgradedTowerUnlocked)
                {
                    upgradeButtonText.text = $"升级 - ${towerData.upgradeCost}";
                    upgradeButton.interactable = true;
                }
                else
                {
                    upgradeButtonText.text = "升级炮塔未解锁";
                    upgradeButton.interactable = false;
                }
            }
            else
            {
                upgradeButton.gameObject.SetActive(false);
            }
        }
        
        if (sellButton != null && sellButtonText != null)
        {
            sellButtonText.text = $"出售 - ${towerData.sellValue}";
        }
        
        if (mainCamera != null)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(tower.transform.position);
            RectTransform rectTransform = GetComponent<RectTransform>();
            Canvas canvas = GetComponentInParent<Canvas>();
            
            if (rectTransform != null)
            {
                rectTransform.pivot = new Vector2(0.5f, 1f);
                
                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);
                    rectTransform.anchoredPosition = localPoint + uiPositionOffset;
                }
                else
                {
                    rectTransform.position = screenPos + uiPositionOffset;
                }
            }
        }
        
        IsUpgradeUIShowing = true;
        gameObject.SetActive(true);
    }
    
    private void TogglePreview()
    {
        if (previewTower != null)
        {
            ClearPreview();
        }
        else
        {
            ShowPreview();
        }
    }
    
    private void ShowPreview()
    {
        ClearPreview();
        
        if (currentTowerData == null || currentTowerData.upgradedTowerPrefab == null)
        {
            return;
        }
        
        TowerClickHandler.ClearTowerRange();
        
        ShowOriginalTowerRange();
        
        Vector3 towerPos = currentTower.transform.position;
        
        previewTower = Instantiate(currentTowerData.upgradedTowerPrefab, towerPos, Quaternion.identity);
        previewTower.name = "UpgradePreviewTower";
        
        SpriteRenderer[] spriteRenderers = previewTower.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color color = sr.color;
            color.a = 0.5f;
            sr.color = color;
            sr.sortingOrder = 1000;
        }
        
        Tower tower = previewTower.GetComponent<Tower>();
        if (tower != null)
        {
            GameObject rangeObj = new GameObject("UpgradedTowerRange");
            rangeObj.transform.SetParent(previewTower.transform);
            rangeObj.transform.localPosition = Vector3.zero;
            
            upgradedTowerRange = rangeObj.AddComponent<LineRenderer>();
            upgradedTowerRange.startWidth = previewLineWidth;
            upgradedTowerRange.endWidth = previewLineWidth;
            upgradedTowerRange.positionCount = 72;
            upgradedTowerRange.loop = true;
            upgradedTowerRange.material = new Material(Shader.Find("Sprites/Default"));
            upgradedTowerRange.startColor = upgradedTowerRangeColor;
            upgradedTowerRange.endColor = upgradedTowerRangeColor;
            upgradedTowerRange.sortingOrder = 999;
            upgradedTowerRange.useWorldSpace = false;
            
            int segments = 72;
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / (float)segments * 360f;
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * tower.attackRange;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * tower.attackRange;
                upgradedTowerRange.SetPosition(i, new Vector3(x, y, 0));
            }
        }
    }
    
    private void ShowOriginalTowerRange()
    {
        if (currentTower == null)
        {
            return;
        }
        
        GameObject rangeObj = new GameObject("OriginalTowerRange");
        rangeObj.transform.SetParent(currentTower.transform);
        rangeObj.transform.localPosition = Vector3.zero;
        
        originalTowerRange = rangeObj.AddComponent<LineRenderer>();
        originalTowerRange.startWidth = previewLineWidth;
        originalTowerRange.endWidth = previewLineWidth;
        originalTowerRange.positionCount = 72;
        originalTowerRange.loop = true;
        originalTowerRange.material = new Material(Shader.Find("Sprites/Default"));
        originalTowerRange.startColor = originalTowerRangeColor;
        originalTowerRange.endColor = originalTowerRangeColor;
        originalTowerRange.sortingOrder = 998;
        originalTowerRange.useWorldSpace = false;
        
        Tower tower = currentTower.GetComponent<Tower>();
        if (tower != null)
        {
            int segments = 72;
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / (float)segments * 360f;
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * tower.attackRange;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * tower.attackRange;
                originalTowerRange.SetPosition(i, new Vector3(x, y, 0));
            }
        }
    }
    
    private void ClearPreview()
    {
        if (previewTower != null)
        {
            Destroy(previewTower);
            previewTower = null;
        }
        
        if (originalTowerRange != null)
        {
            Destroy(originalTowerRange.gameObject);
            originalTowerRange = null;
        }
        
        upgradedTowerRange = null;
    }
    
    public void HideUI()
    {
        IsUpgradeUIShowing = false;
        gameObject.SetActive(false);
        currentTower = null;
        currentTowerData = null;
        TowerClickHandler.ClearTowerRange();
        ClearPreview();
    }
    
    private void UpgradeTower()
    {
        if (currentTower == null || currentTowerData == null)
        {
            HideUI();
            return;
        }
        
        if (currentTowerData.upgradedTowerPrefab == null)
        {
            HideUI();
            return;
        }
        
        TowerData upgradedTowerData = FindUpgradedTowerData(currentTowerData);
        
        if (upgradedTowerData != null)
        {
            if (TowerUnlockManager.instance != null)
            {
                TowerUnlockManager.instance.RegisterAndCheckTower(upgradedTowerData);
                
                if (!TowerUnlockManager.instance.IsTowerUnlocked(upgradedTowerData.towerName))
                {
                    Debug.LogWarning("升级后的炮塔未解锁！请先在商店解锁。");
                    HideUI();
                    return;
                }
            }
        }
        
        if (GameManager.Instance.SpendMoney(currentTowerData.upgradeCost))
        {
            GridManager gridManager = FindObjectOfType<GridManager>();
            if (gridManager != null)
            {
                Vector3 towerPos = gridManager.GetWorldPosFromGrid(currentGridPos.x, currentGridPos.y);
                Destroy(currentTower.gameObject);
                
                GameObject unitManager = GameObject.Find("UnitManager");
                Transform parentTransform = unitManager != null ? unitManager.transform : null;
                
                GameObject newTowerObj = Instantiate(currentTowerData.upgradedTowerPrefab, towerPos, Quaternion.identity, parentTransform);
                
                TowerOnGrid newTowerOnGrid = newTowerObj.AddComponent<TowerOnGrid>();
                if (upgradedTowerData != null)
                {
                    newTowerOnGrid.SetTowerData(upgradedTowerData);
                }
                else
                {
                    newTowerOnGrid.SetTowerData(currentTowerData);
                }
            }
        }
        
        HideUI();
    }
    
    private TowerData FindUpgradedTowerData(TowerData currentData)
    {
        TowerPlacement towerPlacement = FindObjectOfType<TowerPlacement>();
        if (towerPlacement == null)
        {
            return null;
        }
        
        foreach (TowerData towerData in towerPlacement.towerTypes)
        {
            if (towerData != null && towerData.towerPrefab != null)
            {
                if (towerData.towerPrefab == currentData.upgradedTowerPrefab)
                {
                    return towerData;
                }
            }
        }
        
        return null;
    }
    
    private void SellTower()
    {
        if (currentTower == null || currentTowerData == null)
        {
            HideUI();
            return;
        }
        
        GameManager.Instance.AddMoney(currentTowerData.sellValue);
        
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.SetCellState(currentGridPos.x, currentGridPos.y, CellType.Walkable);
        }
        
        Destroy(currentTower.gameObject);
        ZombieMovement.RefreshAllZombiePaths();
        
        HideUI();
    }
}
