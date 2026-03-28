using UnityEngine;

[System.Serializable]
public class LevelData
{
    [Header("关卡基本信息")]
    public string levelName = "第1关";
    public Sprite levelIcon;
    public Sprite backgroundImage;
    public bool isUnlocked = true;
    
    [Header("关卡设置")]
    public int startingMoney = 1000;
    public int startingLives = 10;
    
    [Header("波数配置")]
    public WaveData[] waves;
    
    [Header("胜利奖励")]
    public RewardItem[] winRewards;
    
    [Header("失败奖励")]
    public RewardItem[] loseRewards;
}
