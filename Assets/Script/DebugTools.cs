using UnityEngine;
/// <summary>
/// 调试工具脚本
/// 用于开发和测试时的快速操作
/// </summary>
public class DebugTools : MonoBehaviour
{
    [Header("调试控制")]
    public bool enableDebug = true; // 是否启用调试功能
    
    [Header("调试功能")]
    public int testZombieCount = 5; // 测试僵尸数量
    
    private void Update()
    {
        if (!enableDebug) return;

        // 快速胜利 (F1)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            QuickWin();
        }

        // 快速失败 (F2)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            QuickLose();
        }

        // 测试成就弹出 (F6)
        if (Input.GetKeyDown(KeyCode.F6))
        {
            TestAchievementPopup();
        }
    }
    
    /// <summary>
    /// 快速胜利
    /// </summary>
    public void QuickWin()
    {
        Debug.Log("=== 快速胜利触发 ===");
        
        if (LevelManager.instance == null)
        {
            Debug.LogError("❌ LevelManager实例不存在！");
            return;
        }
        
        // 重置关卡状态
        LevelManager.instance.ResetLevel();
        
        // 标记僵尸生成完成
        Debug.Log($"✅ 标记僵尸生成完成，总数量: {testZombieCount}");
        LevelManager.instance.SetSpawnComplete(testZombieCount);
        
        // 模拟所有僵尸被处理
        Debug.Log($"✅ 模拟处理所有僵尸: {testZombieCount}");
        for (int i = 0; i < testZombieCount; i++)
        {
            LevelManager.instance.AddKillCount();
        }
        
        Debug.Log("=== 快速胜利完成 ===");
    }
    
    /// <summary>
    /// 快速失败
    /// </summary>
    public void QuickLose()
    {
        Debug.Log("=== 快速失败触发 ===");
        if (LevelManager.instance == null)
        {
            return;
        }
        
        LevelManager.instance.LevelLose();
        Debug.Log("=== 快速失败完成 ===");
    }

    /// <summary>
    /// 测试成就弹出
    /// </summary>
    public void TestAchievementPopup()
    {
        Debug.Log("=== 测试成就弹出 ===");

        if (AchievementManager.instance == null)
        {
            Debug.LogError("❌ AchievementManager实例不存在！");
            return;
        }

        if (AchievementPopup.instance == null)
        {
            Debug.LogError("❌ AchievementPopup实例不存在！请在场景中创建成就弹窗预制体");
            return;
        }

        var achievements = AchievementManager.instance.GetIncompleteAchievements();
        if (achievements.Count > 0)
        {
            AchievementPopup.instance.ShowAchievement(achievements[0]);
            Debug.Log($"✅ 显示成就弹窗: {achievements[0].achievementName}");
        }
        else
        {
            var allAchievements = AchievementManager.instance.GetAllAchievements();
            if (allAchievements.Count > 0)
            {
                AchievementPopup.instance.ShowAchievement(allAchievements[0]);
                Debug.Log($"✅ 所有成就已完成，测试显示第一个成就: {allAchievements[0].achievementName}");
            }
            else
            {
                Debug.LogWarning("⚠️ 没有成就数据！");
            }
        }

        Debug.Log("=== 测试成就弹出完成 ===");
    }
}