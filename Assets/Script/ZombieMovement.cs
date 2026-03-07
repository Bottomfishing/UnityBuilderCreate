using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 僵尸移动逻辑（挂载到僵尸预制体）
/// </summary>
public class ZombieMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 2f;
    public GameObject spawnPoint;
    public GameObject endPoint;
    
    [Header("生命值设置")]
    public int damageToPlayer = 1;
    
    private GridManager gridManager;
    private Pathfinding pathfinding;
    private List<Vector3> path;
    private int currentPathIndex;
    private bool isCalculatingPath;
    private Vector3 lastPos; // 记录上一帧位置，判断是否回头
    
    [Header("减速设置")]
    private bool isSlowed = false;
    private float slowDuration = 0f;
    private float slowMultiplier = 0.5f; // 减速到50%
    private float originalMoveSpeed; // 保存原始速度
    
    [Header("颜色设置")]
    public Color slowedColor = new Color(0.5f, 0.8f, 1f, 1f); // 浅蓝色
    public Color frozenColor = new Color(0f, 1f, 1f, 1f); // 青蓝色
    private Color originalColor;
    private SpriteRenderer zombieSpriteRenderer;
    
    [Header("冰冻设置")]
    private bool isFrozen = false;
    private float frozenDuration = 0f;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<Pathfinding>();
        // 设置僵尸标签
        gameObject.tag = "Zombie";
    }

    private void Start()
    {
        originalMoveSpeed = moveSpeed;
        zombieSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (zombieSpriteRenderer != null)
        {
            originalColor = zombieSpriteRenderer.color;
        }
        CalculatePath();
        lastPos = transform.position; // 初始化上一帧位置
    }

    private void Update()
    {
        if (isFrozen)
        {
            frozenDuration -= Time.deltaTime;
            if (frozenDuration <= 0)
            {
                EndFreeze();
            }
            return;
        }
        
        if (isSlowed)
        {
            slowDuration -= Time.deltaTime;
            if (slowDuration <= 0)
            {
                EndSlow();
            }
        }
        
        MoveAlongPath();
        CheckBackwardMove(); // 检测并阻止回头走
    }

    public void ApplySlow(float duration, float multiplier = 0.5f)
    {
        slowDuration = duration;
        slowMultiplier = multiplier;
        isSlowed = true;
        moveSpeed = originalMoveSpeed * slowMultiplier;
        
        if (zombieSpriteRenderer != null)
        {
            zombieSpriteRenderer.color = slowedColor;
        }
    }

    private void EndSlow()
    {
        isSlowed = false;
        moveSpeed = originalMoveSpeed;
        
        if (zombieSpriteRenderer != null)
        {
            zombieSpriteRenderer.color = originalColor;
        }
    }
    
    public void FreezeZombie(float duration)
    {
        frozenDuration = duration;
        isFrozen = true;
        
        if (zombieSpriteRenderer != null)
        {
            zombieSpriteRenderer.color = frozenColor;
        }
    }
    
    private void EndFreeze()
    {
        isFrozen = false;
        
        if (zombieSpriteRenderer != null)
        {
            zombieSpriteRenderer.color = originalColor;
        }
    }

    public void CalculatePath()
    {
        if (isCalculatingPath) return;

        if (spawnPoint == null || endPoint == null)
        {
            return;
        }

        isCalculatingPath = true;

        Vector2Int startGridPos = gridManager.GetGridPosFromWorld(transform.position);
        Vector2Int endGridPos = gridManager.GetGridPosFromWorld(endPoint.transform.position);

        path = pathfinding.FindPath(startGridPos, endGridPos);
        currentPathIndex = 0;

        isCalculatingPath = false;
    }

    // 只横竖走，绝不斜走
    private void MoveAlongPath()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            // 尝试重新计算路径
            CalculatePath();
            return;
        }

        Vector3 target = path[currentPathIndex];
        target.z = 0;

        Vector3 pos = transform.position;
        pos.z = 0;

        // 检查当前位置是否在不可通行的格子上，如果是则移到最近的可通行格子
        if (!IsPositionWalkable(pos))
        {
            MoveToNearestWalkable(pos);
            CalculatePath();
            return;
        }

        // 检查目标路径点是否在不可通行的格子上
        if (!IsPositionWalkable(target))
        {
            CalculatePath();
            return;
        }

        // 先X后Y，强制横竖走
        bool moved = false;
        
        if (Mathf.Abs(pos.x - target.x) > 0.05f)
        {
            float step = moveSpeed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(pos, new Vector3(target.x, pos.y, 0), step);
            // 检查移动后的位置是否可通行
            if (IsPositionWalkable(newPos))
            {
                transform.position = newPos;
                moved = true;
            }
        }
        
        if (!moved && Mathf.Abs(pos.y - target.y) > 0.05f)
        {
            float step = moveSpeed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(pos, new Vector3(pos.x, target.y, 0), step);
            // 检查移动后的位置是否可通行
            if (IsPositionWalkable(newPos))
            {
                transform.position = newPos;
                moved = true;
            }
        }
        
        // 如果完全没有移动，重新计算路径
        if (!moved)
        {
            CalculatePath();
        }

        // 到达路径点
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentPathIndex++;
            
            if (currentPathIndex >= path.Count)
            {
                // 僵尸到达终点，减少生命值
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoseLife(damageToPlayer);
                }
                
                // 通知LevelManager僵尸到达终点
                if (LevelManager.instance != null)
                {
                    LevelManager.instance.AddZombieReachedEnd();
                }
                
                Destroy(gameObject);
            }
        }
    }
    
    // 移动到最近的可通行格子
    private void MoveToNearestWalkable(Vector3 currentPos)
    {
        Vector2Int currentGridPos = gridManager.GetGridPosFromWorld(currentPos);
        
        // 检查周围8个方向的格子
        int[,] directions = { {0,1}, {1,0}, {0,-1}, {-1,0}, {1,1}, {1,-1}, {-1,-1}, {-1,1} };
        
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int x = currentGridPos.x + directions[i, 0];
            int y = currentGridPos.y + directions[i, 1];
            
            // 检查边界
            if (x >= 0 && x < gridManager.gridWidth && y >= 0 && y < gridManager.gridHeight)
            {
                if (gridManager.GetCellState(x, y) == CellType.Walkable)
                {
                    // 移到这个可通行格子
                    Vector3 walkablePos = gridManager.GetWorldPosFromGrid(x, y);
                    transform.position = walkablePos;
                    return;
                }
            }
        }
        
        // 如果周围没有可通行格子，尝试移到起点
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
        }
    }
    
    // 检查位置是否可通行
    private bool IsPositionWalkable(Vector3 position)
    {
        Vector2Int gridPos = gridManager.GetGridPosFromWorld(position);
        CellType cellState = gridManager.GetCellState(gridPos.x, gridPos.y);
        return cellState == CellType.Walkable;
    }
    


    // 核心：检测并阻止回头走
    private void CheckBackwardMove()
    {
        if (endPoint == null) return;

        // 计算“上一帧位置→终点”的距离 和 “当前位置→终点”的距离
        float lastDistance = Vector3.Distance(lastPos, endPoint.transform.position);
        float currentDistance = Vector3.Distance(transform.position, endPoint.transform.position);

        // 如果当前距离 > 上一帧距离 → 僵尸在往回走
        if (currentDistance > lastDistance + 0.1f) // 加0.1容错，避免微小偏移误判
        {
            // 立刻重新计算路径（用当前位置，优先往终点走）
            CalculatePath();
            // 强制回到上一帧位置，避免回头
            transform.position = lastPos;
        }

        // 更新上一帧位置
        lastPos = transform.position;
    }

    // 优化刷新逻辑：只算当前位置到终点的路径，不是起点到终点
    public static void RefreshAllZombiePaths()
    {
        ZombieMovement[] zombies = FindObjectsOfType<ZombieMovement>();
        float delay = 0.05f;
        foreach (var z in zombies)
        {
            if (!z.isCalculatingPath)
            {
                z.CancelInvoke(nameof(z.CalculatePath));
                z.Invoke(nameof(z.CalculatePath), delay);
                delay += 0.05f;
            }
        }
    }
}