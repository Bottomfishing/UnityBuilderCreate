using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [Header("UI组件")]
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button startButton;
    public Text levelNameText;
    public Image levelIconImage;
    public Text levelStatusText;
    
    [Header("关卡数据")]
    public LevelData[] levels;
    
    private int currentLevelIndex = 0;
    private const string UnlockedLevelsKey = "UnlockedLevels";
    private const string TotalLevelsKey = "TotalLevels";
    
    private void Start()
    {
        if (leftArrowButton != null)
        {
            leftArrowButton.onClick.AddListener(OnLeftArrowClick);
        }
        
        if (rightArrowButton != null)
        {
            rightArrowButton.onClick.AddListener(OnRightArrowClick);
        }
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }
        
        SaveTotalLevels();
        LoadUnlockedLevels();
        UpdateUI();
    }
    
    private void SaveTotalLevels()
    {
        if (levels != null)
        {
            PlayerPrefs.SetInt(TotalLevelsKey, levels.Length);
            PlayerPrefs.Save();
        }
    }
    
    private void LoadUnlockedLevels()
    {
        int unlockedCount = PlayerPrefs.GetInt(UnlockedLevelsKey, 1);
        
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].isUnlocked = i < unlockedCount;
        }
    }
    
    private void UpdateUI()
    {
        if (levels == null || levels.Length == 0)
        {
            return;
        }
        
        LevelData currentLevel = levels[currentLevelIndex];
        
        if (levelNameText != null)
        {
            levelNameText.text = currentLevel.levelName;
        }
        
        if (levelIconImage != null && currentLevel.levelIcon != null)
        {
            levelIconImage.sprite = currentLevel.levelIcon;
        }
        
        if (levelStatusText != null)
        {
            if (currentLevel.isUnlocked)
            {
                levelStatusText.text = "已解锁";
                levelStatusText.color = Color.green;
            }
            else
            {
                levelStatusText.text = "未解锁";
                levelStatusText.color = Color.gray;
            }
        }
        
        bool canGoLeft = currentLevelIndex > 0;
        bool canGoRight = currentLevelIndex < levels.Length - 1;
        
        if (leftArrowButton != null)
        {
            leftArrowButton.interactable = canGoLeft;
        }
        
        if (rightArrowButton != null)
        {
            rightArrowButton.interactable = canGoRight;
        }
        
        if (startButton != null)
        {
            startButton.interactable = currentLevel.isUnlocked;
        }
    }
    
    private void OnLeftArrowClick()
    {
        if (currentLevelIndex > 0)
        {
            currentLevelIndex--;
            UpdateUI();
        }
    }
    
    private void OnRightArrowClick()
    {
        if (currentLevelIndex < levels.Length - 1)
        {
            currentLevelIndex++;
            UpdateUI();
        }
    }
    
    private void OnStartButtonClick()
    {
        Debug.Log("=== [验证] OnStartButtonClick 被调用 ===");
        
        if (levels == null || currentLevelIndex >= levels.Length)
        {
            Debug.LogWarning("=== [验证] levels 为空或索引超出范围 ===");
            return;
        }
        
        LevelData currentLevel = levels[currentLevelIndex];
        Debug.Log($"=== [验证] 选择关卡: {currentLevel.levelName}, 已解锁: {currentLevel.isUnlocked}, Waves: {(currentLevel.waves != null ? currentLevel.waves.Length : 0)} ===");
        
        if (!currentLevel.isUnlocked)
        {
            Debug.LogWarning("=== [验证] 关卡未解锁 ===");
            return;
        }
        
        // 消耗10点体力
        if (ResourceManager.instance != null)
        {
            if (!ResourceManager.instance.SpendEnergy(10))
            {
                Debug.LogWarning("=== [验证] 体力不足 ===");
                return;
            }
        }
        
        LevelDataContainer.selectedLevelData = currentLevel;
        Debug.Log($"=== [验证] 设置 selectedLevelData ===");
        
        SceneSwitch sceneSwitch = FindObjectOfType<SceneSwitch>();
        if (sceneSwitch != null)
        {
            Debug.Log("=== [验证] 使用 SceneSwitch 加载场景 ===");
            sceneSwitch.LoadScene("LevelScene");
        }
        else
        {
            Debug.Log("=== [验证] 使用 SceneManager 加载场景 ===");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
        }
    }
    
    public static void UnlockNextLevel()
    {
        int unlockedCount = PlayerPrefs.GetInt(UnlockedLevelsKey, 1);
        int totalLevels = PlayerPrefs.GetInt(TotalLevelsKey, 0);
        
        if (unlockedCount < totalLevels)
        {
            unlockedCount++;
            PlayerPrefs.SetInt(UnlockedLevelsKey, unlockedCount);
            PlayerPrefs.Save();
        }
    }
}

public static class LevelDataContainer
{
    public static LevelData selectedLevelData;
}
