using UnityEngine;
using UnityEngine.UI;

public class WaveStarterUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button startButton;
    public Image countdownCircle;
    public Text countdownText;
    public Text bonusText;
    public Text waveNumberText;
    
    [Header("Visual Settings")]
    public Color readyColor = Color.green;
    public Color waitingColor = Color.yellow;
    public Color urgentColor = Color.red;
    
    [Header("Start Game Text")]
    [Tooltip("第一波之前显示的文字")]
    public string startGameText = "点击开始";
    
    private float totalWaitTime;
    private float currentWaitTime;
    private bool isCounting;
    private bool isGameStartMode;
    private int currentBonusGold;
    private WaveStarterSettings settings;
    private bool isInitialized = false;
    
    public System.Action OnEarlyStart;
    public System.Action OnCountdownComplete;
    
    private void Awake()
    {
        DoInitialize();
    }
    
    public void DoInitialize()
    {
        if (isInitialized) return;
        isInitialized = true;
        
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartButtonClick);
        }
        
        if (countdownCircle != null)
        {
            Button circleBtn = countdownCircle.GetComponent<Button>();
            if (circleBtn == null)
                circleBtn = countdownCircle.gameObject.AddComponent<Button>();
            circleBtn.onClick.RemoveAllListeners();
            circleBtn.onClick.AddListener(OnStartButtonClick);
        }
    }
    
    public void Initialize(WaveStarterSettings waveSettings)
    {
        settings = waveSettings;
    }
    
    public void ShowGameStart()
    {
        if (!isInitialized)
        {
            gameObject.SetActive(true);
            if (!isInitialized)
                DoInitialize();
        }
        
        isGameStartMode = true;
        isCounting = false;
        currentBonusGold = 0;
        
        if (waveNumberText != null)
            waveNumberText.text = startGameText;
        
        if (countdownCircle != null)
        {
            countdownCircle.fillAmount = 1f;
            countdownCircle.color = readyColor;
        }
        
        if (countdownText != null)
            countdownText.text = "";
        
        if (bonusText != null)
            bonusText.gameObject.SetActive(false);
        
        gameObject.SetActive(true);
    }
    
    public void ShowStartWave(int waveNumber, float waitTime)
    {
        if (!isInitialized)
        {
            gameObject.SetActive(true);
            if (!isInitialized)
                DoInitialize();
        }
        
        isGameStartMode = false;
        totalWaitTime = waitTime;
        currentWaitTime = waitTime;
        isCounting = true;
        
        if (waveNumberText != null)
            waveNumberText.text = "第 " + waveNumber + " 波";
        
        UpdateUI();
        gameObject.SetActive(true);
    }
    
    private void Update()
    {
        if (!isCounting || isGameStartMode) return;
        
        currentWaitTime -= Time.deltaTime;
        
        if (currentWaitTime <= 0f)
        {
            currentWaitTime = 0f;
            isCounting = false;
            OnCountdownComplete?.Invoke();
        }
        
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (isGameStartMode) return;
        
        if (countdownCircle != null)
        {
            float progress = totalWaitTime > 0 ? currentWaitTime / totalWaitTime : 0;
            countdownCircle.fillAmount = progress;
            countdownCircle.color = progress > 0.5f ? readyColor
                : (progress > 0.2f ? waitingColor : urgentColor);
        }
        
        if (countdownText != null)
            countdownText.text = Mathf.CeilToInt(currentWaitTime).ToString();
        
        UpdateBonusDisplay();
    }
    
    private void UpdateBonusDisplay()
    {
        if (isGameStartMode)
        {
            if (bonusText != null)
                bonusText.gameObject.SetActive(false);
            return;
        }
        
        currentBonusGold = CalculateBonusGold();
        
        if (settings == null || !settings.showBonusHint || bonusText == null)
        {
            if (bonusText != null)
                bonusText.gameObject.SetActive(false);
            return;
        }
        
        if (currentBonusGold > 0)
        {
            bonusText.text = "+" + currentBonusGold;
            bonusText.gameObject.SetActive(true);
        }
        else
        {
            bonusText.gameObject.SetActive(false);
        }
    }
    
    private int CalculateBonusGold()
    {
        if (isGameStartMode || settings == null || settings.earlyStartRewards == null)
            return 0;
        
        foreach (EarlyStartReward reward in settings.earlyStartRewards)
        {
            if (currentWaitTime >= reward.timeThreshold)
                return reward.bonusGold;
        }
        
        return 0;
    }
    
    private void OnStartButtonClick()
    {
        if (isGameStartMode)
        {
            Hide();
            OnEarlyStart?.Invoke();
            return;
        }
        
        if (!isCounting) return;
        
        isCounting = false;
        
        if (currentBonusGold > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(currentBonusGold);
        }
        
        Hide();
        OnEarlyStart?.Invoke();
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void ForceComplete()
    {
        isCounting = false;
        Hide();
    }
}
