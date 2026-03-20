using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementUI : MonoBehaviour
{
    [Header("UI组件")]
    public Transform content;
    public GameObject achievementItemPrefab;
    public Text titleText;
    public Button closeButton;

    [Header("进度条")]
    public Image overallProgressFill;
    public Text overallProgressText;

    private List<AchievementItem> achievementItems = new List<AchievementItem>();

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

    private void Start()
    {
        Debug.Log("AchievementUI Start");
        
        if (AchievementManager.instance != null)
        {
            AchievementManager.instance.OnAchievementCompleted += OnAchievementCompleted;
            AchievementManager.instance.OnProgressUpdated += OnProgressUpdated;
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (AchievementManager.instance != null)
        {
            AchievementManager.instance.OnAchievementCompleted -= OnAchievementCompleted;
            AchievementManager.instance.OnProgressUpdated -= OnProgressUpdated;
        }
    }

    public void Show()
    {
        Debug.Log("显示成就面板");
        gameObject.SetActive(true);
        RefreshAchievements();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RefreshAchievements()
    {
        ClearAchievementItems();

        if (AchievementManager.instance == null)
        {
            Debug.LogWarning("AchievementManager.instance 为空！");
            return;
        }

        List<AchievementData> achievements = AchievementManager.instance.GetAllAchievements();
        Debug.Log("成就数量：" + achievements.Count);

        if (achievementItemPrefab == null)
        {
            Debug.LogWarning("成就预制体为空！请设置 Achievement Item Prefab");
            return;
        }

        if (content == null)
        {
            Debug.LogWarning("Content 为空！请设置 Content");
            return;
        }

        foreach (AchievementData achievement in achievements)
        {
            CreateAchievementItem(achievement);
        }

        UpdateOverallProgress();
    }

    private void CreateAchievementItem(AchievementData achievement)
    {
        if (achievementItemPrefab == null || content == null) return;

        GameObject itemObj = Instantiate(achievementItemPrefab, content);
        AchievementItem item = itemObj.GetComponent<AchievementItem>();

        if (item != null)
        {
            item.Setup(achievement);
            achievementItems.Add(item);
        }
    }

    private void ClearAchievementItems()
    {
        foreach (AchievementItem item in achievementItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        achievementItems.Clear();
    }

    private void UpdateOverallProgress()
    {
        if (AchievementManager.instance == null) return;

        List<AchievementData> achievements = AchievementManager.instance.GetAllAchievements();
        int completedCount = 0;
        int totalCount = achievements.Count;

        foreach (AchievementData achievement in achievements)
        {
            if (achievement.isCompleted)
            {
                completedCount++;
            }
        }

        if (overallProgressText != null)
        {
            overallProgressText.text = $"{completedCount}/{totalCount}";
        }

        if (overallProgressFill != null)
        {
            float progress = totalCount > 0 ? (float)completedCount / totalCount : 0f;
            overallProgressFill.fillAmount = progress;
        }
    }

    private void OnAchievementCompleted(AchievementData achievement)
    {
        RefreshAchievements();
    }

    private void OnProgressUpdated(string achievementId, int currentValue, int targetValue)
    {
        AchievementItem item = achievementItems.Find(i => i.achievementData.achievementId == achievementId);
        if (item != null)
        {
            item.UpdateProgress(currentValue, targetValue);
        }

        UpdateOverallProgress();
    }
}
