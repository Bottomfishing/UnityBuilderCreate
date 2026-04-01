using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public int currentWave = 0;
    public Text waveText;
    
    [Header("Spawn Point")]
    public GameObject spawnPoint;
    
    [Header("WaveStarter UI")]
    public WaveStarterUI waveStarterUI;
    public WaveStarterSettings waveStarterSettings;
    
    private WaveData[] waves;
    public int TotalWaves => waves != null ? waves.Length : 0;
    
    private float timer;
    private int currentWaveSpawnCount = 0;
    private int totalWaveZombies = 0;
    private int currentZombieTypeIndex = 0;
    private int[] remainingZombies;
    private bool isSpawning = false;
    private bool allowStartWave = false;
    private bool isGameStarted = false;
    
    private void Start()
    {
        Debug.Log("[ZS] Start");
        
        LoadLevelData();
        ResetSpawner();
        
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        
        if (TutorialManager.instance != null && TutorialManager.instance.IsTutorialActive)
        {
            Debug.Log("[ZS] Tutorial active, skip");
            return;
        }
        
        LevelManager.instance?.StartTimer();
        
        // 查找 UI（inactive prefab 也能找到）
        if (waveStarterUI == null)
        {
            WaveStarterUI[] all = FindObjectsOfType<WaveStarterUI>(true);
            if (all.Length > 0) waveStarterUI = all[0];
        }
        Debug.Log("[ZS] waveStarterUI=" + (waveStarterUI != null));
        // 如果没有配置，创建默认奖励

        if (waveStarterSettings == null)

        {

            waveStarterSettings = new WaveStarterSettings();

        }



        // 延迟一帧开始

        StartCoroutine(DelayedShowFirstWaveUI());
    }
    
    private System.Collections.IEnumerator DelayedShowFirstWaveUI()
    {
        yield return null;
        
        if (!isGameStarted)
        {
            isGameStarted = true;
            ShowWaveUI();
        }
    }
    
    // ============================================================
    // 统一入口：显示波次倒计时 UI
    // ============================================================
    private void ShowWaveUI()
    {
        Debug.Log("[ZS] ShowWaveUI wave=" + (currentWave + 1) + " waveStarterUI=" + (waveStarterUI != null));
        
        if (waveStarterUI != null)
        {
            waveStarterUI.Initialize(waveStarterSettings);
            float waitTime = (waveStarterSettings != null) ? waveStarterSettings.waveWaitTime : 10f;
            waveStarterUI.OnEarlyStart -= OnUI_EarlyStart;
            waveStarterUI.OnEarlyStart += OnUI_EarlyStart;
            waveStarterUI.OnCountdownComplete -= OnUI_CountdownDone;
            waveStarterUI.OnCountdownComplete += OnUI_CountdownDone;
            waveStarterUI.ShowStartWave(currentWave + 1, waitTime);
        }
        else
        {
            Debug.LogWarning("[ZS] No WaveStarterUI! Starting wave immediately.");
            StartNextWave();
        }
    }
    
    // ============================================================
    // UI 回调：玩家点击提前开始
    // ============================================================
    private void OnUI_EarlyStart()
    {
        Debug.Log("[ZS] OnUI_EarlyStart");
        waveStarterUI?.Hide();
        StartNextWave();
    }
    
    // ============================================================
    // UI 回调：倒计时结束
    // ============================================================
    private void OnUI_CountdownDone()
    {
        Debug.Log("[ZS] OnUI_CountdownDone");
        waveStarterUI?.ForceComplete();
        StartNextWave();
    }
    
    // ============================================================
    // 正式生成僵尸
    // ============================================================
    private void StartNextWave()
    {
        Debug.Log("[ZS] StartNextWave " + (currentWave + 1));
        allowStartWave = true;
        StartWave();
    }
    
    public void SafeStartWave()
    {
        allowStartWave = true;
        StartWave();
    }
    
    public void StartWave()
    {
        if (!allowStartWave) return;
        allowStartWave = false;
        
        if (waves == null || currentWave >= waves.Length)
        {
            Debug.LogWarning("[ZS] Invalid wave: currentWave=" + currentWave + " Total=" + TotalWaves);
            return;
        }
        
        // 隐藏倒计时 UI
        waveStarterUI?.Hide();
        
        WaveData wave = waves[currentWave];
        currentWaveSpawnCount = 0;
        totalWaveZombies = CalculateTotalZombies(wave);
        currentZombieTypeIndex = 0;
        
        if (wave.zombies != null)
        {
            remainingZombies = new int[wave.zombies.Length];
            for (int i = 0; i < wave.zombies.Length; i++)
                if (wave.zombies[i] != null)
                    remainingZombies[i] = wave.zombies[i].count;
        }
        else return;
        
        isSpawning = true;
        timer = 0;
        UpdateWaveText();
        
        LevelManager.instance?.ResetWave(totalWaveZombies);
    }
    
    public void SetWaveNumber(int waveNumber)
    {
        currentWave = waveNumber - 1;
    }
    
    private void Update()
    {
        if (TutorialManager.instance != null && TutorialManager.instance.IsTutorialActive)
            return;
        
        if (isSpawning && currentWaveSpawnCount < totalWaveZombies)
        {
            timer += Time.deltaTime;
            float interval = (waves != null && currentWave < waves.Length)
                ? waves[currentWave].spawnInterval : 2f;
            
            if (timer >= interval)
            {
                SpawnZombie();
                timer = 0;
            }
        }
    }
    
    private int CalculateTotalZombies(WaveData wave)
    {
        int total = 0;
        if (wave.zombies != null)
            foreach (var z in wave.zombies)
                if (z != null) total += z.count;
        return total;
    }
    
    private void SpawnZombie()
    {
        if (waves == null || currentWave >= waves.Length || spawnPoint == null) return;
        WaveData wave = waves[currentWave];
        if (wave.zombies == null || wave.zombies.Length == 0) return;
        
        int idx = GetNextZombieIndex(wave);
        if (idx < 0) return;
        
        GameObject prefab = wave.zombies[idx].zombiePrefab;
        if (prefab == null) return;
        
        Transform parent = null;
        GameObject um = GameObject.Find("UnitManager");
        if (um != null) parent = um.transform;
        
        GameObject zombie = Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity, parent);
        
        ZombieMovement zMove = zombie.GetComponent<ZombieMovement>();
        if (zMove != null)
        {
            if (zMove.spawnPoint == null) zMove.spawnPoint = spawnPoint;
            GameObject ep = GameObject.Find("EndPoint");
            if (ep != null && zMove.endPoint == null) zMove.endPoint = ep;
        }
        
        remainingZombies[idx]--;
        currentWaveSpawnCount++;
        
        if (currentWaveSpawnCount >= totalWaveZombies)
        {
            isSpawning = false;
            LevelManager.instance?.SetSpawnComplete(totalWaveZombies);
        }
    }
    
    private int GetNextZombieIndex(WaveData wave)
    {
        if (wave.zombies == null || wave.zombies.Length == 0) return -1;
        float r = Random.Range(0f, 1f);
        return r < wave.mixChance ? GetRandomZ(wave) : GetSequentialZ(wave);
    }
    
    private int GetRandomZ(WaveData wave)
    {
        int total = 0;
        for (int i = 0; i < wave.zombies.Length; i++)
            if (wave.zombies[i]?.zombiePrefab != null) total += remainingZombies[i];
        if (total <= 0) return -1;
        int pos = Random.Range(0, total);
        int cur = 0;
        for (int i = 0; i < wave.zombies.Length; i++)
        {
            if (wave.zombies[i]?.zombiePrefab == null) continue;
            cur += remainingZombies[i];
            if (pos < cur) return i;
        }
        return -1;
    }
    
    private int GetSequentialZ(WaveData wave)
    {
        while (currentZombieTypeIndex < wave.zombies.Length)
        {
            WaveZombie z = wave.zombies[currentZombieTypeIndex];
            if (z?.zombiePrefab != null && remainingZombies[currentZombieTypeIndex] > 0)
                return currentZombieTypeIndex;
            currentZombieTypeIndex++;
        }
        return -1;
    }
    
    private void UpdateWaveText()
    {
        if (waveText != null && waves != null)
            waveText.text = "第 " + (currentWave + 1) + " / " + waves.Length + " 波";
    }
    
    public void OnWaveComplete()
    {
        Debug.Log("[ZS] OnWaveComplete");
        currentWave++;
        
        if (currentWave >= TotalWaves)
        {
            Debug.Log("[ZS] All waves done!");
            return;
        }
        
        ShowWaveUI(); // 下一波 UI
    }
    
    private void LoadLevelData()
    {
        if (LevelDataContainer.selectedLevelData != null)
            waves = LevelDataContainer.selectedLevelData.waves;
    }
    
    public void ResetSpawner()
    {
        currentWave = 0;
        currentWaveSpawnCount = 0;
        totalWaveZombies = 0;
        currentZombieTypeIndex = 0;
        remainingZombies = null;
        isSpawning = false;
        timer = 0;
        allowStartWave = false;
        isGameStarted = false;
        UpdateWaveText();
    }
}
