using UnityEngine;

[System.Serializable]
public class LevelData
{
    [Header("关卡基本信息")]
    public string levelName = "第1关";
    public Sprite levelIcon;
    public Sprite backgroundImage;
    public bool isUnlocked = true;
    
    [Header("波数配置")]
    public WaveData[] waves;
    
    [Header("胜利奖励")]
    public RewardItem[] winRewards;
    
    [Header("失败奖励")]
    public RewardItem[] loseRewards;
}
