using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [Header("成就列表")]
    public List<AchievementData> achievements = new List<AchievementData>();

    private Dictionary<string, AchievementData> achievementDict = new Dictionary<string, AchievementData>();
    private PlayerAchievementData playerData;

    public delegate void AchievementCompletedDelegate(AchievementData achievement);
    public event AchievementCompletedDelegate OnAchievementCompleted;

    public delegate void AchievementProgressDelegate(string achievementId, int currentValue, int targetValue);
    public event AchievementProgressDelegate OnProgressUpdated;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null, false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeAchievements();
        LoadPlayerData();
    }

    private void InitializeAchievements()
    {
        achievementDict.Clear();
        foreach (AchievementData achievement in achievements)
        {
            if (!achievementDict.ContainsKey(achievement.achievementId))
            {
                achievementDict.Add(achievement.achievementId, achievement);
            }
        }
    }

    public void UpdateProgress(AchievementType type, int amount)
    {
        foreach (AchievementData achievement in achievements)
        {
            if (achievement.type == type && !achievement.isCompleted)
            {
                achievement.currentValue += amount;
                
                if (OnProgressUpdated != null)
                {
                    OnProgressUpdated(achievement.achievementId, achievement.currentValue, achievement.targetValue);
                }

                if (achievement.currentValue >= achievement.targetValue)
                {
                    CompleteAchievement(achievement);
                }
            }
        }
        
        SavePlayerData();
    }

    public void SetProgress(AchievementType type, int value)
    {
        foreach (AchievementData achievement in achievements)
        {
            if (achievement.type == type && !achievement.isCompleted)
            {
                achievement.currentValue = value;
                
                if (OnProgressUpdated != null)
                {
                    OnProgressUpdated(achievement.achievementId, achievement.currentValue, achievement.targetValue);
                }

                if (achievement.currentValue >= achievement.targetValue)
                {
                    CompleteAchievement(achievement);
                }
            }
        }
        
        SavePlayerData();
    }

    private void CompleteAchievement(AchievementData achievement)
    {
        if (achievement.isCompleted) return;

        achievement.isCompleted = true;
        
        if (achievement.reward != null)
        {
            if (achievement.reward.coins > 0 && ResourceManager.instance != null)
            {
                ResourceManager.instance.AddCoins(achievement.reward.coins);
            }
            if (achievement.reward.diamonds > 0 && ResourceManager.instance != null)
            {
                ResourceManager.instance.AddDiamonds(achievement.reward.diamonds);
            }
            if (achievement.reward.energy > 0 && ResourceManager.instance != null)
            {
                ResourceManager.instance.AddEnergy(achievement.reward.energy);
            }
        }

        if (AchievementPopup.instance != null)
        {
            AchievementPopup.instance.ShowAchievement(achievement);
        }

        if (OnAchievementCompleted != null)
        {
            OnAchievementCompleted(achievement);
        }

        SavePlayerData();
    }

    public List<AchievementData> GetAllAchievements()
    {
        return achievements;
    }

    public List<AchievementData> GetCompletedAchievements()
    {
        return achievements.FindAll(a => a.isCompleted);
    }

    public List<AchievementData> GetIncompleteAchievements()
    {
        return achievements.FindAll(a => !a.isCompleted);
    }

    public AchievementData GetAchievement(string achievementId)
    {
        if (achievementDict.ContainsKey(achievementId))
        {
            return achievementDict[achievementId];
        }
        return null;
    }

    public float GetProgressPercentage(AchievementData achievement)
    {
        if (achievement.targetValue == 0) return 0f;
        return Mathf.Clamp01((float)achievement.currentValue / achievement.targetValue);
    }

    private void SavePlayerData()
    {
        PlayerAchievementData data = new PlayerAchievementData();
        
        foreach (AchievementData achievement in achievements)
        {
            if (achievement.isCompleted)
            {
                data.completedAchievements.Add(achievement.achievementId);
            }
            
            ProgressEntry entry = new ProgressEntry();
            entry.achievementId = achievement.achievementId;
            entry.progressValue = achievement.currentValue;
            data.progressList.Add(entry);
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("AchievementData", json);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        string json = PlayerPrefs.GetString("AchievementData", "");
        
        if (!string.IsNullOrEmpty(json))
        {
            playerData = JsonUtility.FromJson<PlayerAchievementData>(json);
            
            foreach (string achievementId in playerData.completedAchievements)
            {
                AchievementData achievement = GetAchievement(achievementId);
                if (achievement != null)
                {
                    achievement.isCompleted = true;
                    achievement.currentValue = achievement.targetValue;
                }
            }

            foreach (ProgressEntry entry in playerData.progressList)
            {
                AchievementData achievement = GetAchievement(entry.achievementId);
                if (achievement != null)
                {
                    achievement.currentValue = entry.progressValue;
                }
            }
        }
    }

    public void ResetProgress()
    {
        foreach (AchievementData achievement in achievements)
        {
            achievement.isCompleted = false;
            achievement.currentValue = 0;
        }
        
        SavePlayerData();
    }
}
