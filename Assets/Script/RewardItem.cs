using UnityEngine;

public enum RewardType
{
    Coins,
    Diamonds,
    Energy,
    Other
}

[System.Serializable]
public class RewardItem
{
    public RewardType type;
    public int amount;
    public Sprite icon;
}
