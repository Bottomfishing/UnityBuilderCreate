using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TowerPlacement : MonoBehaviour
{
    [Header("基础设置")]
    public GridManager gridManager;
    public Camera mainCamera;
    public Pathfinding pathfinding;

    [Header("炮塔配置")]
    public List<TowerData> towerTypes = new List<TowerData>();

    [Header("UI设置")]
    public GameObject towerSelectionUI;
    public RectTransform towerButtonContainer;
    public GameObject towerButtonPrefab;
    public Button closeButton;

    [Header("预览设置")]
    public Material previewMaterial;
    public Color previewRangeColor = new Color(0, 1, 0, 0.3f);
    public Color blockedPathColor = new Color(1, 0, 0, 0.3f);
    public float previewRangeLineWidth = 2f;

    [Header("UI位置设置")]
    public Vector2 uiPositionOffset = new Vector2(0, 0);

    private Vector2Int selectedGridPos;
    private List<GameObject> towerButtons = new List<GameObject>();
    private bool isUIShowing = false;
    private GameObject previewTower;
    private LineRenderer previewRange;
    private TowerData currentlyPreviewedTower;

    private void Awake()
    {
        if (towerSelectionUI != null)
        {
            towerSelectionUI.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTowerSelectionUI);
        }
        
        if (pathfinding == null)
        {
            pathfinding = FindObjectOfType<Pathfinding>();
        }
    }

    private void Start()
    {
        if (TowerUnlockManager.instance != null)
        {
            TowerUnlockManager.instance.InitializeDefaultTowers(towerTypes);
        }
    }

    void LateUpdate()
    {
        if (IsTutorialActive())
        {
            if (isUIShowing)
            {
                CloseTowerSelectionUI();
            }
            return;
        }
        
        if (IsSkillActive())
        {
            if (isUIShowing)
            {
                CloseTowerSelectionUI();
            }
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (SkillManager.skillClickProcessed)
            {
                return;
            }
            
            if (isUIShowing)
            {
                if (!IsPointerOverUIElement(towerSelectionUI))
                {
                    CloseTowerSelectionUI();
                }
            }
            else if (!IsGamePaused() && !TowerUpgradeUI.IsUpgradeUIShowing)
            {
                ShowTowerSelectionUI();
            }
        }
    }
    
    private bool IsTutorialActive()
    {
        if (TutorialManager.instance != null)
        {
            return TutorialManager.instance.IsTutorialActive;
        }
        return false;
    }
    
    private bool IsSkillActive()
    {
        if (SkillManager.instance != null)
        {
            return SkillManager.instance.IsSkillActive();
        }
        return false;
    }
    
    private bool IsPositionInGrid(Vector3 worldPos)
    {
        float minX = gridManager.gridOrigin.x;
        float maxX = gridManager.gridOrigin.x + gridManager.gridWidth * gridManager.cellSize;
        float minY = gridManager.gridOrigin.y;
        float maxY = gridManager.gridOrigin.y + gridManager.gridHeight * gridManager.cellSize;
        
        return worldPos.x >= minX && worldPos.x <= maxX && 
               worldPos.y >= minY && worldPos.y <= maxY;
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

    private bool IsGamePaused()
    {
        if (LevelManager.instance == null)
        {
            return false;
        }

        return LevelManager.instance.isLevelWin ||
               LevelManager.instance.levelTipParent == null ||
               LevelManager.instance.levelTipParent.activeSelf;
    }

    private void ShowTowerSelectionUI()
    {
        if (IsGamePaused())
        {
            return;
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        if (!IsPositionInGrid(mouseWorldPos))
        {
            return;
        }
        
        selectedGridPos = gridManager.GetGridPosFromWorld(mouseWorldPos);

        if (gridManager.GetCellState(selectedGridPos.x, selectedGridPos.y) != CellType.Walkable)
        {
            return;
        }

        Vector3 gridWorldPos = gridManager.GetWorldPosFromGrid(selectedGridPos.x, selectedGridPos.y);
        Vector2 screenPos = mainCamera.WorldToScreenPoint(gridWorldPos);

        if (towerSelectionUI != null)
        {
            RectTransform uiRect = towerSelectionUI.GetComponent<RectTransform>();
            Canvas canvas = towerSelectionUI.GetComponentInParent<Canvas>();

            uiRect.pivot = new Vector2(0.5f, 0f);

            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);
                uiRect.anchoredPosition = localPoint + uiPositionOffset;
            }
            else
            {
                uiRect.position = screenPos + uiPositionOffset;
            }

            GenerateTowerButtons();
            towerSelectionUI.SetActive(true);
            isUIShowing = true;

            foreach (TowerData towerData in towerTypes)
            {
                if (towerData != null && towerData.towerPrefab != null && towerData.isBuildable)
                {
                    PreviewTower(towerData);
                    break;
                }
            }
        }
    }

    private void GenerateTowerButtons()
    {
        foreach (GameObject button in towerButtons)
        {
            Destroy(button);
        }
        towerButtons.Clear();

        foreach (TowerData towerData in towerTypes)
        {
            if (towerData != null && towerData.towerPrefab != null && towerData.isBuildable)
            {
                bool isUnlocked = CheckTowerUnlocked(towerData);

                GameObject buttonObj = Instantiate(towerButtonPrefab, towerButtonContainer);
                towerButtons.Add(buttonObj);

                SetupTowerButton(buttonObj, towerData, isUnlocked);
            }
        }
    }

    private bool CheckTowerUnlocked(TowerData towerData)
    {
        if (TowerUnlockManager.instance == null)
        {
            return true;
        }
        return TowerUnlockManager.instance.IsTowerUnlocked(towerData.towerName);
    }

    private void SetupTowerButton(GameObject buttonObj, TowerData towerData, bool isUnlocked)
    {
        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            if (isUnlocked)
            {
                buttonText.text = $"{towerData.towerName} - ${towerData.cost}";
            }
            else
            {
                buttonText.text = $"{towerData.towerName} - 未解锁";
            }
        }

        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            if (isUnlocked)
            {
                button.onClick.AddListener(() => SelectAndPlaceTower(towerData));
            }
            else
            {
                button.interactable = false;
            }
        }

        Button[] allButtons = buttonObj.GetComponentsInChildren<Button>();
        if (allButtons.Length >= 2)
        {
            Button previewButton = allButtons[1];
            if (previewButton != null)
            {
                if (isUnlocked)
                {
                    previewButton.onClick.AddListener(() => PreviewTower(towerData));
                }
                else
                {
                    previewButton.interactable = false;
                }
            }
        }
    }

    private void PreviewTower(TowerData towerData)
    {
        ClearPreview();

        if (towerData == null || towerData.towerPrefab == null)
        {
            return;
        }

        Vector3 towerPos = gridManager.GetWorldPosFromGrid(selectedGridPos.x, selectedGridPos.y);
        towerPos.z = 0;

        // 检查放置炮塔后是否还有路径
        bool canPlace = true;
        if (pathfinding != null)
        {
            canPlace = pathfinding.CanPlaceTowerAt(selectedGridPos.x, selectedGridPos.y);
        }

        previewTower = Instantiate(towerData.towerPrefab, towerPos, Quaternion.identity);
        previewTower.name = "PreviewTower";

        SpriteRenderer[] spriteRenderers = previewTower.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color color = canPlace ? sr.color : Color.red;
            color.a = 0.5f;
            sr.color = color;
            sr.sortingOrder = 1000;

            if (previewMaterial != null)
            {
                sr.material = previewMaterial;
            }
        }

        Tower tower = previewTower.GetComponent<Tower>();
        if (tower != null)
        {
            GameObject rangeObj = new GameObject("PreviewRange");
            rangeObj.transform.SetParent(previewTower.transform);
            rangeObj.transform.localPosition = Vector3.zero;

            previewRange = rangeObj.AddComponent<LineRenderer>();
            previewRange.startWidth = previewRangeLineWidth;
            previewRange.endWidth = previewRangeLineWidth;
            previewRange.positionCount = 72;
            previewRange.loop = true;
            previewRange.material = new Material(Shader.Find("Sprites/Default"));
            Color rangeColor = canPlace ? previewRangeColor : blockedPathColor;
            previewRange.startColor = rangeColor;
            previewRange.endColor = rangeColor;
            previewRange.sortingOrder = 999;
            previewRange.useWorldSpace = false;

            int segments = 72;
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / (float)segments * 360f;
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * tower.attackRange;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * tower.attackRange;
                previewRange.SetPosition(i, new Vector3(x, y, 0));
            }
        }

        currentlyPreviewedTower = towerData;
    }

    private void ClearPreview()
    {
        if (previewTower != null)
        {
            Destroy(previewTower);
            previewTower = null;
        }

        previewRange = null;
        currentlyPreviewedTower = null;
    }

    private void SelectAndPlaceTower(TowerData towerData)
    {
        if (IsGamePaused())
        {
            CloseTowerSelectionUI();
            return;
        }

        if (towerData.towerPrefab == null)
        {
            CloseTowerSelectionUI();
            return;
        }

        if (GameManager.Instance.SpendMoney(towerData.cost))
        {
            if (gridManager.GetCellState(selectedGridPos.x, selectedGridPos.y) != CellType.Walkable)
            {
                CloseTowerSelectionUI();
                return;
            }
            
            // 检查放置炮塔后是否还有路径
            if (pathfinding != null && !pathfinding.CanPlaceTowerAt(selectedGridPos.x, selectedGridPos.y))
            {
                // 退还金币
                GameManager.Instance.AddMoney(towerData.cost);
                CloseTowerSelectionUI();
                return;
            }

            gridManager.SetCellState(selectedGridPos.x, selectedGridPos.y, CellType.Blocked);
            Vector3 towerPos = gridManager.GetWorldPosFromGrid(selectedGridPos.x, selectedGridPos.y);

            GameObject unitManager = GameObject.Find("UnitManager");
            Transform parentTransform = unitManager != null ? unitManager.transform : null;

            GameObject towerObj = Instantiate(towerData.towerPrefab, towerPos, Quaternion.identity, parentTransform);

            TowerOnGrid towerOnGrid = towerObj.AddComponent<TowerOnGrid>();
            towerOnGrid.SetTowerData(towerData);

            ZombieMovement.RefreshAllZombiePaths();
        }

        CloseTowerSelectionUI();
    }

    public void CloseTowerSelectionUI()
    {
        ClearPreview();
        TowerClickHandler.ClearTowerRange();

        if (towerSelectionUI != null)
        {
            towerSelectionUI.SetActive(false);
        }

        foreach (GameObject button in towerButtons)
        {
            Destroy(button);
        }
        towerButtons.Clear();
        isUIShowing = false;
    }
}

[System.Serializable]
public class TowerData
{
    public string towerName;
    public int cost;
    public int sellValue;
    public int upgradeCost;
    public bool isBuildable = true;
    public bool defaultUnlocked = true;
    public GameObject towerPrefab;
    public GameObject upgradedTowerPrefab;
}
