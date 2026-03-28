using UnityEngine;

[System.Serializable]
public struct EarlyStartReward
{
    [Tooltip("时间阈值（秒），提前多少秒以上可以获得该奖励")]
    public float timeThreshold;
    
    [Tooltip("奖励的金币数量")]
    public int bonusGold;
}

[System.Serializable]
public class WaveStarterSettings
{
    [Header("倒计时设置")]
    [Tooltip("每波之间的等待时间（秒）")]
    public float waveWaitTime = 10f;
    
    [Tooltip("提前开始按钮显示的时机（波次结束前多少秒显示）")]
    public float showEarlyButtonTime = 5f;
    
    [Header("奖励设置")]
    [Tooltip("提前开始的奖励配置，按时间从长到短排序")]
    public EarlyStartReward[] earlyStartRewards = new EarlyStartReward[]
    {
        new EarlyStartReward { timeThreshold = 10f, bonusGold = 50 },
        new EarlyStartReward { timeThreshold = 5f, bonusGold = 30 },
        new EarlyStartReward { timeThreshold = 2f, bonusGold = 15 },
        new EarlyStartReward { timeThreshold = 0f, bonusGold = 5 }
    };
    
    [Header("UI设置")]
    [Tooltip("是否显示奖励金额提示")]
    public bool showBonusHint = true;
}
