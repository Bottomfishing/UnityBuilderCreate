using UnityEngine;
using UnityEngine.UI;

public class WaveStarterUI : MonoBehaviour
{
    [Header("UI组件")]
    public Button startButton;
    public Image countdownCircle;
    public Text countdownText;
    public Text bonusText;
    public Text waveNumberText;
    
    [Header("视觉设置")]
    public Color readyColor = Color.green;
    public Color waitingColor = Color.yellow;
    public Color urgentColor = Color.red;
    
    private float totalWaitTime;
    private float currentWaitTime;
    private bool isCounting;
    private int currentBonusGold;
    private WaveStarterSettings settings;
    
    public System.Action OnEarlyStart;
    
    private void Awake()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }
    }
    
    private void Start()
    {
        if (countdownCircle != null)
        {
            Button circleButton = countdownCircle.GetComponent<Button>();
            if (circleButton == null)
            {
                circleButton = countdownCircle.gameObject.AddComponent<Button>();
            }
            circleButton.onClick.AddListener(OnStartButtonClick);
        }
        
        totalWaitTime = 10f;
        currentWaitTime = 10f;
        isCounting = true;
        
        if (waveNumberText != null)
        {
            waveNumberText.text = "第 1 波";
        }
    }
    
    public void Initialize(WaveStarterSettings waveSettings)
    {
        settings = waveSettings;
    }
    
    public void ShowStartWave(int waveNumber, float waitTime)
    {
        totalWaitTime = waitTime;
        currentWaitTime = waitTime;
        isCounting = true;
        
        if (waveNumberText != null)
        {
            waveNumberText.text = $"第 {waveNumber} 波";
        }
        
        UpdateBonusDisplay();
        UpdateUI();
        Show();
    }
    
    public void ShowEarlyStart(int nextWaveNumber, float remainingTime, float totalTime)
    {
        totalWaitTime = totalTime;
        currentWaitTime = remainingTime;
        isCounting = true;
        
        if (waveNumberText != null)
        {
            waveNumberText.text = $"第 {nextWaveNumber} 波";
        }
        
        UpdateBonusDisplay();
        UpdateUI();
        Show();
    }
    
    private void Update()
    {
        if (isCounting)
        {
            currentWaitTime -= Time.deltaTime;
            
            if (currentWaitTime <= 0f)
            {
                currentWaitTime = 0f;
                isCounting = false;
                Hide();
            }
            
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        if (countdownCircle != null)
        {
            float progress = totalWaitTime > 0 ? currentWaitTime / totalWaitTime : 0;
            countdownCircle.fillAmount = progress;
            
            if (progress > 0.7f)
            {
                countdownCircle.color = readyColor;
            }
            else if (progress > 0.3f)
            {
                countdownCircle.color = waitingColor;
            }
            else
            {
                countdownCircle.color = urgentColor;
            }
        }
        
        if (countdownText != null)
        {
            countdownText.text = Mathf.CeilToInt(currentWaitTime).ToString();
        }
        
        UpdateBonusDisplay();
    }
    
    private void UpdateBonusDisplay()
    {
        if (settings == null || bonusText == null || !settings.showBonusHint)
        {
            if (bonusText != null)
            {
                bonusText.gameObject.SetActive(false);
            }
            return;
        }
        
        currentBonusGold = CalculateBonusGold();
        
        if (currentBonusGold > 0)
        {
            bonusText.text = $"+{currentBonusGold}";
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
        {
            return 0;
        }
        
        foreach (EarlyStartReward reward in settings.earlyStartRewards)
        {
            if (currentWaitTime >= reward.timeThreshold)
            {
                return reward.bonusGold;
            }
        }
        
        return 0;
    }
    
    private void OnStartButtonClick()
    {
        if (!isCounting)
        {
            return;
        }
        
        isCounting = false;
        
        if (currentBonusGold > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(currentBonusGold);
        }
        
        Hide();
        
        if (OnEarlyStart != null)
        {
            OnEarlyStart.Invoke();
        }
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
