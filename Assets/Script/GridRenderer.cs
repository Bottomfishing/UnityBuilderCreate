using UnityEngine;
using System.Collections.Generic;

public class GridRenderer : MonoBehaviour
{
    [Header("网格设置")]

    public GridManager gridManager;    
    [Header("网格样式")]
    public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public float lineWidth = 0.05f;
    
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private GameObject gridLinesContainer;
    
    void Start()
    {
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
        }
        
        if (gridManager == null)
        {
            return;
        }
        
        CreateGridLinesContainer();
        DrawGrid();
    }
    
    void CreateGridLinesContainer()
    {
        gridLinesContainer = new GameObject("GridLines");
        gridLinesContainer.transform.SetParent(transform);
        gridLinesContainer.transform.localPosition = Vector3.zero;
    }
    
    public void DrawGrid()
    {
        if (gridManager == null)
        {
            return;
        }
        
        ClearGrid();
        
        float startX = gridManager.gridOrigin.x;
        float startY = gridManager.gridOrigin.y;
        float cellSize = gridManager.cellSize;
        int gridWidth = gridManager.gridWidth;
        int gridHeight = gridManager.gridHeight;
        
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(startX + x * cellSize, startY, 0);
            Vector3 end = new Vector3(startX + x * cellSize, startY + gridHeight * cellSize, 0);
            CreateLineRenderer(start, end);
        }
        
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(startX, startY + y * cellSize, 0);
            Vector3 end = new Vector3(startX + gridWidth * cellSize, startY + y * cellSize, 0);
            CreateLineRenderer(start, end);
        }
    }
    
    void CreateLineRenderer(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(gridLinesContainer.transform);
        
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = gridColor;
        lineRenderer.material = mat;
        
        lineRenderer.startColor = gridColor;
        lineRenderer.endColor = gridColor;
        lineRenderer.sortingOrder = -100;
        
        lineRenderers.Add(lineRenderer);
    }
    
    void ClearGrid()
    {
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
            }
        }
        lineRenderers.Clear();
    }
}
