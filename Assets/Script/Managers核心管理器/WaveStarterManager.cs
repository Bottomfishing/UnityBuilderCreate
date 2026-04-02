using UnityEngine;

/// <summary>
/// WaveStarterManager — 已简化
/// 
/// 当前架构：
/// - ZombieSpawner 直接持有 WaveStarterUI，不再走这里
/// - 这个脚本可以保留备用（如需要全局波次管理），暂时不再被调用
/// - 如果不需要，可以从场景里删除这个组件
/// </summary>
public class WaveStarterManager : MonoBehaviour
{
    [Header("Settings")]
    public WaveStarterSettings settings;
    
    [Header("References")]
    public WaveStarterUI waveStarterUI;
    public ZombieSpawner zombieSpawner;
    
    private bool isGameStarted = false;
    private int nextWaveNumber = 1;
    
    private void Awake()
    {
        if (settings == null)
            settings = new WaveStarterSettings();
        
        if (waveStarterUI == null)
            waveStarterUI = GetComponent<WaveStarterUI>();
    }
    
    private void Start()
    {
        TryInitializeUI();
        
        if (zombieSpawner != null)
        {
            // 指向 ZombieSpawner，但 ZombieSpawner 不再指向这里
            // 这是单向引用，没问题
        }
    }
    
    private void TryInitializeUI()
    {
        if (waveStarterUI == null)
        {
            waveStarterUI = GetComponent<WaveStarterUI>();
            if (waveStarterUI == null)
            {
                WaveStarterUI[] all = FindObjectsOfType<WaveStarterUI>(true);
                if (all.Length > 0) waveStarterUI = all[0];
            }
        }
        
        if (waveStarterUI != null)
        {
            waveStarterUI.DoInitialize();
            waveStarterUI.Initialize(settings);
            waveStarterUI.OnEarlyStart -= HandleEarlyStart;
            waveStarterUI.OnEarlyStart += HandleEarlyStart;
            waveStarterUI.OnCountdownComplete -= HandleCountdownComplete;
            waveStarterUI.OnCountdownComplete += HandleCountdownComplete;
        }
        else
        {
            Debug.LogError("[WS-Mgr] WaveStarterUI NOT FOUND!");
        }
    }
    
    public void StartGame()
    {
        if (isGameStarted) return;
        isGameStarted = true;
        nextWaveNumber = 1;
        TryInitializeUI();
        ShowWaveUI();
    }
    
    public void OnCurrentWaveStarted()
    {
        waveStarterUI?.Hide();
    }
    
    public void OnCurrentWaveComplete()
    {
        if (!isGameStarted) return;
        nextWaveNumber++;
        TryInitializeUI();
        ShowWaveUI();
    }
    
    private void ShowWaveUI()
    {
        if (waveStarterUI != null)
            waveStarterUI.ShowStartWave(nextWaveNumber, settings.waveWaitTime);
        else
            Debug.LogError("[WS-Mgr] No WaveStarterUI!");
    }
    
    private void HandleCountdownComplete()
    {
        waveStarterUI?.ForceComplete();
        // 不自动开始，由外部控制
    }
    
    private void HandleEarlyStart()
    {
        waveStarterUI?.Hide();
        // 不自动开始，由外部控制
    }
    
    public void ResetManager()
    {
        isGameStarted = false;
        nextWaveNumber = 1;
        waveStarterUI?.Hide();
    }
}
