using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AchievementData
{
    public string achievementId;
    public string achievementName;
    public string description;
    public Sprite icon;
    public AchievementType type;
    public int targetValue;
    public int currentValue;
    public bool isCompleted;
    public RewardData reward;

    public AchievementData()
    {
        isCompleted = false;
        currentValue = 0;
    }
}

[System.Serializable]
public enum AchievementType
{
    KillZombies,           // 击杀僵尸数量
    PassLevels,            // 通关关卡数量
    UseSkills,             // 使用技能次数
    BuildTowers,           // 建造炮塔数量
    CollectCoins,          // 收集金币总量
    WatchAds,              // 观看广告次数
    PlayTime,              // 累计游戏时间
    PerfectClear,          // 无漏怪通关
    EndlessWave,           // 无尽模式波数
    UseGems                // 使用钻石次数
}

[System.Serializable]
public class RewardData
{
    public Sprite coinsIcon;
    public int coins;
    public Sprite diamondsIcon;
    public int diamonds;
    public Sprite energyIcon;
    public int energy;
}

[System.Serializable]
public class ProgressEntry
{
    public string achievementId;
    public int progressValue;
}

[System.Serializable]
public class PlayerAchievementData
{
    public List<string> completedAchievements = new List<string>();
    public List<ProgressEntry> progressList = new List<ProgressEntry>();
}
