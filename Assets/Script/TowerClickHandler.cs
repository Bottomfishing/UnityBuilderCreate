using UnityEngine;

public class TowerClickHandler : MonoBehaviour
{
    [Header("基础设置")]
    public Camera mainCamera;
    public TowerUpgradeUI upgradeUI;
    public TowerPlacement towerPlacement;
    public Color placedTowerRangeColor = new Color(0, 0.8f, 1, 0.3f);
    public float placedTowerRangeLineWidth = 0.1f;
    
    private static GameObject currentRangeDisplay;
    private static LineRenderer currentRangeLine;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsGamePaused() || TowerUpgradeUI.IsUpgradeUIShowing)
            {
                return;
            }
            
            CheckTowerClick();
        }
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
    
    private void CheckTowerClick()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(mouseWorldPos);
        
        foreach (Collider2D collider in hitColliders)
        {
            Tower tower = collider.GetComponent<Tower>();
            if (tower != null)
            {
                ShowTowerRange(tower);
                ShowUpgradeUI(tower);
                return;
            }
        }
        
        ClearTowerRange();
    }
    
    private void ShowTowerRange(Tower tower)
    {
        ClearTowerRange();
        
        if (tower == null)
        {
            return;
        }
        
        GameObject rangeObj = new GameObject("PlacedTowerRange");
        rangeObj.transform.SetParent(tower.transform);
        rangeObj.transform.localPosition = Vector3.zero;
        
        LineRenderer lineRenderer = rangeObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = placedTowerRangeLineWidth;
        lineRenderer.endWidth = placedTowerRangeLineWidth;
        lineRenderer.positionCount = 72;
        lineRenderer.loop = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = placedTowerRangeColor;
        lineRenderer.endColor = placedTowerRangeColor;
        lineRenderer.sortingOrder = 999;
        lineRenderer.useWorldSpace = false;
        
        int segments = 72;
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / (float)segments * 360f;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * tower.attackRange;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * tower.attackRange;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
        
        currentRangeDisplay = rangeObj;
        currentRangeLine = lineRenderer;
    }
    
    public static void ClearTowerRange()
    {
        if (currentRangeDisplay != null)
        {
            Destroy(currentRangeDisplay);
            currentRangeDisplay = null;
            currentRangeLine = null;
        }
    }
    
    private void ShowUpgradeUI(Tower tower)
    {
        if (upgradeUI == null)
        {
            return;
        }
        
        TowerOnGrid towerOnGrid = tower.GetComponent<TowerOnGrid>();
        if (towerOnGrid == null || towerOnGrid.towerData == null)
        {
            return;
        }
        
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            return;
        }
        
        Vector2Int gridPos = gridManager.GetGridPosFromWorld(tower.transform.position);
        upgradeUI.ShowUI(tower, towerOnGrid.towerData, gridPos, mainCamera);
    }
}
