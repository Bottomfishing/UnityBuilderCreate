using System.Collections.Generic;
using UnityEngine;

// A*寻路节点（改回class，解决循环布局问题）
public class PathNode
{
    public int x;          
    public int y;          
    public int gCost;      
    public int hCost;      
    public int fCost => gCost + hCost; 
    public PathNode parent;

    public PathNode(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.gCost = int.MaxValue; // 初始化为最大值，避免重复赋值
        this.hCost = 0;
        this.parent = null; // class用null初始化父节点
    }
}

public class Pathfinding : MonoBehaviour
{
    private GridManager gridManager;
    private int gridWidth;
    private int gridHeight;
    
    [Header("路径检查设置")]
    public GameObject spawnPoint;
    public GameObject endPoint;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        // 缓存网格尺寸，避免每次都取
        gridWidth = gridManager.gridWidth;
        gridHeight = gridManager.gridHeight;
        
        // 自动查找 spawnPoint 和 endPoint
        if (spawnPoint == null)
        {
            spawnPoint = GameObject.Find("SpawnPoint");
        }
        if (endPoint == null)
        {
            endPoint = GameObject.Find("EndPoint");
        }
    }

    // 核心：优化后的A*寻路（减少重复计算+提前终止）
    public List<Vector3> FindPath(Vector2Int startGridPos, Vector2Int endGridPos)
    {
        int startX = startGridPos.x;
        int startY = startGridPos.y;
        int endX = endGridPos.x;
        int endY = endGridPos.y;

        // 快速判断：起点/终点不可通行，直接返回空
        if (gridManager.GetCellState(startX, startY) == CellType.Blocked ||
            gridManager.GetCellState(endX, endY) == CellType.Blocked)
        {
            return new List<Vector3>();
        }

        // 用数组代替List，减少GC（网格最大尺寸8*10=80，足够用）
        PathNode[,] nodeGrid = new PathNode[gridWidth, gridHeight];
        bool[,] closedList = new bool[gridWidth, gridHeight]; // 用布尔数组代替HashSet，更快
        List<PathNode> openList = new List<PathNode>(64); // 预设容量，减少扩容

        // 初始化节点网格
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                nodeGrid[x, y] = new PathNode(x, y);
            }
        }

        // 初始化起点节点
        PathNode startNode = nodeGrid[startX, startY];
        startNode.gCost = 0;
        startNode.hCost = CalculateHCost(startNode.x, startNode.y, endX, endY);
        nodeGrid[startX, startY] = startNode;
        openList.Add(startNode);

        // 核心寻路循环（加最大迭代次数，防止死循环）
        int maxIterations = gridWidth * gridHeight * 2;
        int iterations = 0;

        while (openList.Count > 0 && iterations < maxIterations)
        {
            iterations++;

            // 找fCost最小的节点（优化：只遍历一次）
            PathNode currentNode = GetLowestFCostNode(openList);
            openList.Remove(currentNode);
            closedList[currentNode.x, currentNode.y] = true;

            // 到达终点，直接回溯路径
            if (currentNode.x == endX && currentNode.y == endY)
            {
                return RetracePath(nodeGrid, startNode, currentNode);
            }

            // 遍历相邻节点（只算上下左右，减少计算）
            CheckNeighbor(nodeGrid, openList, closedList, currentNode, 0, 1, endX, endY); // 上
            CheckNeighbor(nodeGrid, openList, closedList, currentNode, 0, -1, endX, endY); // 下
            CheckNeighbor(nodeGrid, openList, closedList, currentNode, -1, 0, endX, endY); // 左
            CheckNeighbor(nodeGrid, openList, closedList, currentNode, 1, 0, endX, endY); // 右
        }

        // 没找到路径/迭代超限，返回空
        return new List<Vector3>();
    }

    // 优化：单独处理相邻节点，减少重复代码
    private void CheckNeighbor(PathNode[,] nodeGrid, List<PathNode> openList, bool[,] closedList,
                               PathNode currentNode, int xOffset, int yOffset, int endX, int endY)
    {
        int neighborX = currentNode.x + xOffset;
        int neighborY = currentNode.y + yOffset;

        // 边界+不可通行+已关闭 快速判断
        if (neighborX < 0 || neighborX >= gridWidth || neighborY < 0 || neighborY >= gridHeight ||
            gridManager.GetCellState(neighborX, neighborY) == CellType.Blocked ||
            closedList[neighborX, neighborY])
        {
            return;
        }

        PathNode neighborNode = nodeGrid[neighborX, neighborY];
        int newGCost = currentNode.gCost + 1;

        // 只有新路径更优，才更新
        if (newGCost < neighborNode.gCost)
        {
            neighborNode.gCost = newGCost;
            neighborNode.hCost = CalculateHCost(neighborX, neighborY, endX, endY);
            neighborNode.parent = currentNode;
            nodeGrid[neighborX, neighborY] = neighborNode;

            // 不在开放列表才添加
            if (!openList.Contains(neighborNode))
            {
                openList.Add(neighborNode);
            }
        }
    }

    // 回溯路径（优化：移除重复方向的路径点，让路径更直）
private List<Vector3> RetracePath(PathNode[,] nodeGrid, PathNode startNode, PathNode endNode)
{
    List<Vector3> rawPath = new List<Vector3>(32);
    PathNode currentNode = endNode;

    // 先收集原始路径
    while (currentNode != null && (currentNode.x != startNode.x || currentNode.y != startNode.y))
    {
        rawPath.Add(gridManager.GetWorldPosFromGrid(currentNode.x, currentNode.y));
        currentNode = currentNode.parent;
    }
    rawPath.Reverse();

    // 优化：移除连续同方向的路径点（核心）
    List<Vector3> smoothPath = new List<Vector3>();
    if (rawPath.Count == 0) return smoothPath;

    // 保留第一个点
    smoothPath.Add(rawPath[0]);
    Vector2 lastDirection = Vector2.zero;

    // 遍历路径点，只保留方向变化的点
    for (int i = 1; i < rawPath.Count; i++)
    {
        Vector2 currentDirection = new Vector2(
            rawPath[i].x - smoothPath[smoothPath.Count - 1].x,
            rawPath[i].y - smoothPath[smoothPath.Count - 1].y
        ).normalized;

        // 方向变化时，才保留这个路径点
        if (currentDirection != lastDirection && currentDirection != Vector2.zero)
        {
            smoothPath.Add(rawPath[i]);
            lastDirection = currentDirection;
        }
    }

    // 强制保留最后一个点（终点）
    if (smoothPath[smoothPath.Count - 1] != rawPath[rawPath.Count - 1])
    {
        smoothPath.Add(rawPath[rawPath.Count - 1]);
    }

    return smoothPath;
}

    // 计算HCost（极简版）
    private int CalculateHCost(int x, int y, int endX, int endY)
    {
        return Mathf.Abs(x - endX) + Mathf.Abs(y - endY);
    }

    // 找最低FCost节点（优化：减少比较次数）
    private PathNode GetLowestFCostNode(List<PathNode> nodeList)
    {
        PathNode lowestNode = nodeList[0];
        for (int i = 1; i < nodeList.Count; i++)
        {
            if (nodeList[i].fCost < lowestNode.fCost)
            {
                lowestNode = nodeList[i];
            }
        }
        return lowestNode;
    }
    
    // 检查放置某个格子后是否还有路径（用于放置炮塔前验证）
    public bool CanPlaceTowerAt(int gridX, int gridY)
    {
        if (spawnPoint == null || endPoint == null)
        {
            return true;
        }
        
        // 获取起点和终点的网格位置
        Vector2Int startGridPos = gridManager.GetGridPosFromWorld(spawnPoint.transform.position);
        Vector2Int endGridPos = gridManager.GetGridPosFromWorld(endPoint.transform.position);
        
        // 保存原格子状态
        CellType originalState = gridManager.GetCellState(gridX, gridY);
        
        // 临时设置为阻塞
        gridManager.SetCellState(gridX, gridY, CellType.Blocked);
        
        // 检查是否还有路径
        List<Vector3> path = FindPath(startGridPos, endGridPos);
        bool hasPath = path != null && path.Count > 0;
        
        // 恢复原状态
        gridManager.SetCellState(gridX, gridY, originalState);
        
        return hasPath;
    }
}