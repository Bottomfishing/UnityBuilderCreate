using UnityEngine;

[System.Serializable]
public class DailyReward
{
    public RewardType rewardType = RewardType.Coins;
    public int amount = 50;
    public Sprite rewardIcon;
}

[System.Serializable]
public class DailyCheckInData
{
    public DailyReward[] dailyRewards = new DailyReward[7];
}
