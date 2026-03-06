using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("UI组件")]
    public GameObject levelTipParent;
    public Text winText;
    public Text loseText;
    public Text pauseText;
    public GameObject pauseMask;
    
    [Header("按钮")]
    public Button againButton;
    public Button resumeButton;
    public Button backButton;
    
    [Header("奖励UI组件")]
    public GameObject rewardPanel;
    public Transform rewardContainer;
    public Transform contentContainer;
    public GameObject rewardItemPrefab;

    [Header("关卡状态")]
    public bool isLevelWin = false;
    public bool isSpawnComplete = false;
    public bool isPaused = false;
    
    private RewardItem[] currentWinRewards;
    private RewardItem[] currentLoseRewards;
    private bool shouldCheckForWin = false;

    private int totalZombies = 0;
    private int processedZombies = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Update()
    {
        if (shouldCheckForWin && !isLevelWin)
        {
            ZombieMovement[] zombies = FindObjectsOfType<ZombieMovement>();
            if (zombies.Length == 0)
            {
                shouldCheckForWin = false;
                
                ZombieSpawner zombieSpawner = FindObjectOfType<ZombieSpawner>();
                if (zombieSpawner != null && zombieSpawner.currentWave + 1 < zombieSpawner.waves.Length)
                {
                    zombieSpawner.OnWaveComplete();
                }
                else
                {
                    LevelWin();
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        LoadLevelData();
        ResetLevel();
        FindUIElements();
    }
    
    private void LoadLevelData()
    {
        if (LevelDataContainer.selectedLevelData != null)
        {
            currentWinRewards = LevelDataContainer.selectedLevelData.winRewards;
            currentLoseRewards = LevelDataContainer.selectedLevelData.loseRewards;
        }
    }
    
    private void FindUIElements()
    {
        if (levelTipParent == null)
        {
            GameObject tipObj = GameObject.Find("LevelTip");
            if (tipObj != null)
            {
                levelTipParent = tipObj;
            }
        }
        
        if (winText == null && levelTipParent != null)
        {
            Text winTextComp = levelTipParent.transform.Find("WinText").GetComponent<Text>();
            if (winTextComp != null)
            {
                winText = winTextComp;
            }
        }
        
        if (loseText == null && levelTipParent != null)
        {
            Text loseTextComp = levelTipParent.transform.Find("LoseText").GetComponent<Text>();
            if (loseTextComp != null)
            {
                loseText = loseTextComp;
            }
        }
        
        if (pauseText == null && levelTipParent != null)
        {
            Text pauseTextComp = levelTipParent.transform.Find("PauseText").GetComponent<Text>();
            if (pauseTextComp != null)
            {
                pauseText = pauseTextComp;
            }
        }
        
        if (rewardPanel == null && levelTipParent != null)
        {
            Transform rewardPanelTrans = levelTipParent.transform.Find("RewardPanel");
            if (rewardPanelTrans != null)
            {
                rewardPanel = rewardPanelTrans.gameObject;
            }
        }
        
        if (rewardContainer == null && rewardPanel != null)
        {
            rewardContainer = rewardPanel.transform;
        }
        
        if (contentContainer == null && rewardPanel != null)
        {
            Transform contentTrans = rewardPanel.transform.Find("ContentContainer");
            if (contentTrans != null)
            {
                contentContainer = contentTrans;
            }
            else
            {
                contentContainer = rewardContainer;
            }
        }
        
        if (pauseMask == null)
        {
            GameObject maskObj = GameObject.Find("PauseMask");
            if (maskObj != null)
            {
                pauseMask = maskObj;
            }
        }
    }

    public void AddKillCount()
    {
        if (isLevelWin) return;

        processedZombies++;
        CheckLevelWin();
    }

    public void AddZombieReachedEnd()
    {
        if (isLevelWin) return;

        processedZombies++;
        CheckLevelWin();
    }

    public void SetSpawnComplete(int totalSpawned)
    {
        isSpawnComplete = true;
        totalZombies = totalSpawned;
        CheckLevelWin();
    }
    
    public void ResetWave(int totalZombiesThisWave)
    {
        isSpawnComplete = false;
        totalZombies = totalZombiesThisWave;
        processedZombies = 0;
        shouldCheckForWin = false;
    }

    private void CheckLevelWin()
    {
        if (isSpawnComplete && processedZombies >= totalZombies && !isLevelWin)
        {
            shouldCheckForWin = true;
        }
    }

    private void LevelWin()
    {
        isLevelWin = true;
        
        CloseAllUIs();
        
        LevelSelector.UnlockNextLevel();
        
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
        }
        if (loseText != null)
        {
            loseText.gameObject.SetActive(false);
        }
        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(false);
        }

        ShowRewards(currentWinRewards);
        GiveRewards(currentWinRewards);

        if (pauseMask != null)
        {
            pauseMask.SetActive(true);
        }

        if (levelTipParent != null)
        {
            levelTipParent.SetActive(true);
        }

        // 显示再来一次和返回按钮，隐藏继续按钮
        if (againButton != null)
        {
            againButton.gameObject.SetActive(true);
        }
        if (resumeButton != null)
        {
            resumeButton.gameObject.SetActive(false);
        }
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }

        Time.timeScale = 0;
    }

    public void LevelLose()
    {
        CloseAllUIs();
        
        if (winText != null)
        {
            winText.gameObject.SetActive(false);
        }
        if (loseText != null)
        {
            loseText.gameObject.SetActive(true);
        }
        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(false);
        }

        ShowRewards(currentLoseRewards);
        GiveRewards(currentLoseRewards);

        if (pauseMask != null)
        {
            pauseMask.SetActive(true);
        }

        if (levelTipParent != null)
        {
            levelTipParent.SetActive(true);
        }

        // 显示再来一次和返回按钮，隐藏继续按钮
        if (againButton != null)
        {
            againButton.gameObject.SetActive(true);
        }
        if (resumeButton != null)
        {
            resumeButton.gameObject.SetActive(false);
        }
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }

        Time.timeScale = 0;
    }
    
    public void PauseGame()
    {
        if (isLevelWin || isPaused) return;
        
        isPaused = true;
        CloseAllUIs();
        
        if (winText != null)
        {
            winText.gameObject.SetActive(false);
        }
        if (loseText != null)
        {
            loseText.gameObject.SetActive(false);
        }
        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(true);
            pauseText.text = "可以继续游戏";
        }
        
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }

        if (pauseMask != null)
        {
            pauseMask.SetActive(true);
        }

        if (levelTipParent != null)
        {
            levelTipParent.SetActive(true);
        }

        // 隐藏再来一次按钮，显示继续和返回按钮
        if (againButton != null)
        {
            againButton.gameObject.SetActive(false);
        }
        if (resumeButton != null)
        {
            resumeButton.gameObject.SetActive(true);
        }
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }

        Time.timeScale = 0;
    }
    
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        isPaused = false;
        
        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(false);
        }

        if (pauseMask != null)
        {
            pauseMask.SetActive(false);
        }

        if (levelTipParent != null)
        {
            levelTipParent.SetActive(false);
        }

        Time.timeScale = 1;
    }
    
    public void OnAgainButtonClick()
    {
        // 消耗10点体力
        if (ResourceManager.instance != null)
        {
            if (!ResourceManager.instance.SpendEnergy(10))
            {
                return;
            }
        }
        
        // 重置关卡
        ResetLevel();
        Time.timeScale = 1;
        
        // 隐藏提示框
        if (levelTipParent != null)
        {
            levelTipParent.SetActive(false);
        }
        
        if (pauseMask != null)
        {
            pauseMask.SetActive(false);
        }
    }
    
    public void OnBackButtonClick()
    {
        Time.timeScale = 1;
        
        SceneSwitch sceneSwitch = FindObjectOfType<SceneSwitch>();
        if (sceneSwitch != null)
        {
            sceneSwitch.GoToMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }
    }
    
    private void CloseAllUIs()
    {
        TowerPlacement towerPlacement = FindObjectOfType<TowerPlacement>();
        if (towerPlacement != null)
        {
            towerPlacement.CloseTowerSelectionUI();
        }
        
        TowerUpgradeUI towerUpgradeUI = FindObjectOfType<TowerUpgradeUI>();
        if (towerUpgradeUI != null)
        {
            towerUpgradeUI.HideUI();
        }
        
        TowerClickHandler.ClearTowerRange();
    }
    
    private void ShowRewards(RewardItem[] rewards)
    {
        if (rewardPanel == null || contentContainer == null) return;
        
        rewardPanel.SetActive(true);
        
        foreach (Transform child in contentContainer)
        {
            if (child != contentContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        if (rewards == null || rewards.Length == 0) return;
        
        foreach (RewardItem reward in rewards)
        {
            if (rewardItemPrefab != null)
            {
                GameObject itemObj = Instantiate(rewardItemPrefab, contentContainer);
                RewardDisplayItem displayItem = itemObj.GetComponent<RewardDisplayItem>();
                if (displayItem != null)
                {
                    displayItem.Setup(reward);
                }
            }
        }
    }
    
    private void GiveRewards(RewardItem[] rewards)
    {
        if (rewards == null) return;
        
        foreach (RewardItem reward in rewards)
        {
            switch (reward.type)
            {
                case RewardType.Coins:
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddMoney(reward.amount);
                    }
                    if (ResourceManager.instance != null)
                    {
                        ResourceManager.instance.AddCoins(reward.amount);
                    }
                    break;
                    
                case RewardType.Diamonds:
                    if (ResourceManager.instance != null)
                    {
                        ResourceManager.instance.AddDiamonds(reward.amount);
                    }
                    break;
                    
                case RewardType.Energy:
                    if (ResourceManager.instance != null)
                    {
                        ResourceManager.instance.AddEnergy(reward.amount);
                    }
                    break;
                    
                case RewardType.Other:
                    break;
            }
        }
    }

    public void ResetLevel()
    {
        isLevelWin = false;
        isSpawnComplete = false;
        isPaused = false;
        shouldCheckForWin = false;
        totalZombies = 0;
        processedZombies = 0;
        
        // 清理所有僵尸
        ZombieMovement[] allZombies = FindObjectsOfType<ZombieMovement>();
        foreach (ZombieMovement zombie in allZombies)
        {
            Destroy(zombie.gameObject);
        }
        
        // 清理所有炮塔
        GameObject unitManager = GameObject.Find("UnitManager");
        if (unitManager != null)
        {
            TowerOnGrid[] allTowers = unitManager.GetComponentsInChildren<TowerOnGrid>();
            foreach (TowerOnGrid tower in allTowers)
            {
                Destroy(tower.gameObject);
            }
        }
        
        // 重置GridManager
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.ResetGrid();
        }
        
        // 重置GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameManager();
        }
        
        // 重置僵尸生成器
        ZombieSpawner zombieSpawner = FindObjectOfType<ZombieSpawner>();
        if (zombieSpawner != null)
        {
            zombieSpawner.ResetSpawner();
        }

        if (winText != null)
        {
            winText.gameObject.SetActive(false);
        }
        if (loseText != null)
        {
            loseText.gameObject.SetActive(false);
        }
        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(false);
        }
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
            
            if (contentContainer != null)
            {
                foreach (Transform child in contentContainer)
                {
                    if (child != contentContainer)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
        if (pauseMask != null)
        {
            pauseMask.SetActive(false);
        }
        if (levelTipParent != null)
        {
            levelTipParent.SetActive(false);
        }

        Time.timeScale = 1;
    }
}
