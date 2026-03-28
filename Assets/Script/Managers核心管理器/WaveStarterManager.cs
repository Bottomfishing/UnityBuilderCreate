using UnityEngine;

public class WaveStarterManager : MonoBehaviour
{
    [Header("设置")]
    public WaveStarterSettings settings;
    
    [Header("UI引用")]
    public WaveStarterUI waveStarterUI;
    
    [Header("引用")]
    public ZombieSpawner zombieSpawner;
    
    private bool isGameStarted = false;
    private float currentWaitTime = 0f;
    private bool isWaitingForWave = false;
    private int nextWaveNumber = 0;
    
    private void Awake()
    {
        if (settings == null)
        {
            settings = new WaveStarterSettings();
        }
        
        if (waveStarterUI != null)
        {
            waveStarterUI.Initialize(settings);
            waveStarterUI.OnEarlyStart += OnEarlyStartClicked;
        }
    }
    
    private void Start()
    {
        if (zombieSpawner == null)
        {
            zombieSpawner = FindObjectOfType<ZombieSpawner>();
        }
    }
    
    public void StartGame()
    {
        if (isGameStarted)
        {
            return;
        }
        
        if (waveStarterUI == null)
        {
            return;
        }
        
        isGameStarted = true;
        nextWaveNumber = 1;
        
        ShowStartFirstWave();
    }
    
    private void ShowStartFirstWave()
    {
        if (waveStarterUI != null)
        {
            waveStarterUI.ShowStartWave(nextWaveNumber, settings.waveWaitTime);
        }
        
        isWaitingForWave = true;
        currentWaitTime = settings.waveWaitTime;
    }
    
    public void OnCurrentWaveStarted()
    {
        if (waveStarterUI != null)
        {
            waveStarterUI.Hide();
        }
        
        isWaitingForWave = false;
    }
    
    public void OnCurrentWaveComplete()
    {
        nextWaveNumber++;
        
        if (nextWaveNumber > zombieSpawner.TotalWaves)
        {
            return;
        }
        
        ShowEarlyStartButton();
    }
    
    private void ShowEarlyStartButton()
    {
        if (waveStarterUI != null)
        {
            waveStarterUI.ShowEarlyStart(nextWaveNumber, settings.waveWaitTime, settings.waveWaitTime);
        }
        
        isWaitingForWave = true;
        currentWaitTime = settings.waveWaitTime;
    }
    
    private void Update()
    {
        if (!isWaitingForWave)
        {
            return;
        }
        
        currentWaitTime -= Time.deltaTime;
        
        if (currentWaitTime <= 0f)
        {
            AutoStartNextWave();
        }
    }
    
    private void AutoStartNextWave()
    {
        isWaitingForWave = false;
        
        if (waveStarterUI != null)
        {
            waveStarterUI.ForceComplete();
        }
        
        StartNextWave();
        
        if (zombieSpawner != null)
        {
            OnCurrentWaveStarted();
        }
    }
    
    private void OnEarlyStartClicked()
    {
        isWaitingForWave = false;
        
        if (waveStarterUI != null)
        {
            waveStarterUI.Hide();
        }
        
        StartNextWave();
        
        if (zombieSpawner != null)
        {
            OnCurrentWaveStarted();
        }
    }
    
    private void StartNextWave()
    {
        if (zombieSpawner != null)
        {
            if (!isGameStarted)
            {
                isGameStarted = true;
            }
            zombieSpawner.SafeStartWave();
        }
    }
    
    public void ResetManager()
    {
        isGameStarted = false;
        isWaitingForWave = false;
        currentWaitTime = 0f;
        nextWaveNumber = 0;
        
        if (waveStarterUI != null)
        {
            waveStarterUI.Hide();
        }
    }
}
