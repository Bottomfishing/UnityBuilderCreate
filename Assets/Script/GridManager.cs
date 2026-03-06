using UnityEngine;

public enum CellType
{
    Walkable,
    Blocked
}

public class GridManager : MonoBehaviour
{
    [Header("网格设置")]
    public int gridWidth = 8;
    public int gridHeight = 10;
    public float cellSize = 1f;
    public Vector3 gridOrigin = new Vector3(-4, -5, 0);
    
    [Header("网格显示")]
    public bool showGridInGame = true;
    
    private CellType[,] gridCells;
    
    private void Awake()
    {
        gridCells = new CellType[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridCells[x, y] = CellType.Walkable;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y, 0);
            Vector3 end = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y + gridHeight * cellSize, 0);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(start, end);
        }
        
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(gridOrigin.x, gridOrigin.y + y * cellSize, 0);
            Vector3 end = new Vector3(gridOrigin.x + gridWidth * cellSize, gridOrigin.y + y * cellSize, 0);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(start, end);
        }
        
        if (gridCells != null)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridCells[x, y] == CellType.Blocked)
                    {
                        Vector3 cellCenter = GetWorldPosFromGrid(x, y);
                        Gizmos.color = new Color(1, 0, 0, 0.3f);
                        Gizmos.DrawCube(cellCenter, new Vector3(cellSize * 0.9f, cellSize * 0.9f, 1));
                    }
                }
            }
        }
    }
    
    private void OnGUI()
    {
        if (!showGridInGame) return;
        
        GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.2f);
        
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 startWorld = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y, 0);
            Vector3 endWorld = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y + gridHeight * cellSize, 0);
            
            Vector2 startScreen = Camera.main.WorldToScreenPoint(startWorld);
            Vector2 endScreen = Camera.main.WorldToScreenPoint(endWorld);
            
            startScreen.y = Screen.height - startScreen.y;
            endScreen.y = Screen.height - endScreen.y;
            
            DrawLine(startScreen, endScreen, 1f);
        }
        
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 startWorld = new Vector3(gridOrigin.x, gridOrigin.y + y * cellSize, 0);
            Vector3 endWorld = new Vector3(gridOrigin.x + gridWidth * cellSize, gridOrigin.y + y * cellSize, 0);
            
            Vector2 startScreen = Camera.main.WorldToScreenPoint(startWorld);
            Vector2 endScreen = Camera.main.WorldToScreenPoint(endWorld);
            
            startScreen.y = Screen.height - startScreen.y;
            endScreen.y = Screen.height - endScreen.y;
            
            DrawLine(startScreen, endScreen, 1f);
        }
    }
    
    private void DrawLine(Vector2 start, Vector2 end, float width)
    {
        Vector2 direction = end - start;
        float length = direction.magnitude;
        direction.Normalize();
        
        Rect lineRect = new Rect(start.x, start.y - width / 2, length, width);
        
        Matrix4x4 oldMatrix = GUI.matrix;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GUIUtility.RotateAroundPivot(angle, start);
        
        GUI.DrawTexture(lineRect, Texture2D.whiteTexture);
        
        GUI.matrix = oldMatrix;
    }
    
    public Vector2Int GetGridPosFromWorld(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - gridOrigin.y) / cellSize);
        
        x = Mathf.Clamp(x, 0, gridWidth - 1);
        y = Mathf.Clamp(y, 0, gridHeight - 1);
        
        return new Vector2Int(x, y);
    }
    
    public Vector3 GetWorldPosFromGrid(int x, int y)
    {
        return new Vector3(
            gridOrigin.x + x * cellSize + cellSize / 2,
            gridOrigin.y + y * cellSize + cellSize / 2,
            0
        );
    }
    
    public bool SetCellState(int x, int y, CellType type)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }
        gridCells[x, y] = type;
        return true;
    }
    
    public CellType GetCellState(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return CellType.Blocked;
        }
        return gridCells[x, y];
    }
    
    public void ResetGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridCells[x, y] = CellType.Walkable;
            }
        }
    }
}
