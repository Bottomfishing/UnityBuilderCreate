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
    
    private float totalWaitTime;
    private float currentWaitTime;
    private bool isCounting;
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
        Debug.Log($"WaveStarterUI: Initialized with settings, showBonusHint={settings?.showBonusHint}");
    }
    
    public void ShowStartWave(int waveNumber, float waitTime)
    {
        if (!isInitialized)
        {
            gameObject.SetActive(true);
            if (!isInitialized)
                DoInitialize();
        }
        
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
        if (!isCounting) return;
        
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
        if (settings == null || settings.earlyStartRewards == null)
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
        if (!isCounting) return;
        
        isCounting = false;
        
        if (currentBonusGold > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(currentBonusGold);
            Debug.Log($"WaveStarterUI: Added {currentBonusGold} gold bonus to player");
        }
        else if (currentBonusGold > 0 && GameManager.Instance == null)
        {
            Debug.LogWarning("WaveStarterUI: GameManager.Instance is null, cannot add bonus gold!");
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
