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
        if (levels == null || currentLevelIndex >= levels.Length)
        {
            return;
        }
        
        LevelData currentLevel = levels[currentLevelIndex];
        if (!currentLevel.isUnlocked)
        {
            return;
        }
        
        // 消耗10点体力
        if (ResourceManager.instance != null)
        {
            if (!ResourceManager.instance.SpendEnergy(10))
            {
                return;
            }
        }
        
        LevelDataContainer.selectedLevelData = currentLevel;
        
        SceneSwitch sceneSwitch = FindObjectOfType<SceneSwitch>();
        if (sceneSwitch != null)
        {
            sceneSwitch.LoadScene("LevelScene");
        }
        else
        {
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
