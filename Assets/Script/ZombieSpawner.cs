using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("波数设置")]
    public int currentWave = 0;
    public Text waveText;
    public float waveDelay = 3f;
    
    [Header("基础设置")]
    public GameObject spawnPoint;
    
    private WaveData[] waves;
    
    public int TotalWaves
    {
        get { return waves != null ? waves.Length : 0; }
    }
    private float timer;
    private int currentWaveSpawnCount = 0;
    private int totalWaveZombies = 0;
    private int currentZombieTypeIndex = 0;
    private int[] remainingZombies;
    private bool isSpawning = false;
    private bool isWaitingForNextWave = false;
    private float waveDelayTimer = 0f;
    
    private void Start()
    {
        LoadLevelData();
        ResetSpawner();

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        if (LevelManager.instance != null)
        {
            LevelManager.instance.StartTimer();
        }

        StartWave();
    }

    private void LoadLevelData()
    {
        if (LevelDataContainer.selectedLevelData != null)
        {
            waves = LevelDataContainer.selectedLevelData.waves;
        }
    }
    
    private void Update()
    {
        if (isWaitingForNextWave)
        {
            waveDelayTimer += Time.deltaTime;
            if (waveDelayTimer >= waveDelay)
            {
                isWaitingForNextWave = false;
                StartWave();
            }
            return;
        }
        
        if (isSpawning && currentWaveSpawnCount < totalWaveZombies)
        {
            timer += Time.deltaTime;
            float currentSpawnInterval = GetCurrentWaveInterval();
            if (timer >= currentSpawnInterval)
            {
                SpawnZombie();
                timer = 0;
            }
        }
    }
    
    private float GetCurrentWaveInterval()
    {
        if (waves != null && currentWave < waves.Length)
        {
            return waves[currentWave].spawnInterval;
        }
        return 2f;
    }
    
    private void StartWave()
    {
        if (waves == null || currentWave >= waves.Length)
        {
            return;
        }
        
        WaveData wave = waves[currentWave];
        
        currentWaveSpawnCount = 0;
        totalWaveZombies = CalculateTotalWaveZombies(wave);
        currentZombieTypeIndex = 0;
        
        if (wave.zombies != null)
        {
            remainingZombies = new int[wave.zombies.Length];
            for (int i = 0; i < wave.zombies.Length; i++)
            {
                if (wave.zombies[i] != null)
                {
                    remainingZombies[i] = wave.zombies[i].count;
                }
            }
        }
        else
        {
            return;
        }
        
        isSpawning = true;
        timer = 0;
        
        UpdateWaveText();
        
        if (LevelManager.instance != null)
        {
            LevelManager.instance.ResetWave(totalWaveZombies);
        }
    }
    
    private int CalculateTotalWaveZombies(WaveData wave)
    {
        int total = 0;
        if (wave.zombies != null)
        {
            foreach (WaveZombie zombie in wave.zombies)
            {
                if (zombie != null)
                {
                    total += zombie.count;
                }
            }
        }
        return total;
    }
    
    private void SpawnZombie()
    {
        if (waves == null || currentWave >= waves.Length || spawnPoint == null)
        {
            return;
        }

        WaveData wave = waves[currentWave];
        if (wave.zombies == null || wave.zombies.Length == 0)
        {
            return;
        }

        int selectedIndex = GetNextZombieIndex(wave);

        if (selectedIndex < 0)
        {
            return;
        }

        GameObject selectedPrefab = wave.zombies[selectedIndex].zombiePrefab;
        if (selectedPrefab == null)
        {
            return;
        }

        GameObject unitManager = GameObject.Find("UnitManager");
        Transform parentTransform = unitManager != null ? unitManager.transform : null;

        GameObject zombie = Instantiate(selectedPrefab, spawnPoint.transform.position, Quaternion.identity, parentTransform);
        
        ZombieMovement zombieMove = zombie.GetComponent<ZombieMovement>();
        if (zombieMove != null)
        {
            if (zombieMove.spawnPoint == null)
            {
                zombieMove.spawnPoint = spawnPoint;
            }
            
            GameObject endPointObj = GameObject.Find("EndPoint");
            if (endPointObj != null && zombieMove.endPoint == null)
            {
                zombieMove.endPoint = endPointObj;
            }
        }
        
        remainingZombies[selectedIndex]--;
        currentWaveSpawnCount++;
        
        if (currentWaveSpawnCount >= totalWaveZombies)
        {
            isSpawning = false;
            if (LevelManager.instance != null)
            {
                LevelManager.instance.SetSpawnComplete(totalWaveZombies);
            }
        }
    }
    
    private int GetNextZombieIndex(WaveData wave)
    {
        if (wave.zombies == null || wave.zombies.Length == 0)
        {
            return -1;
        }
        
        float randomValue = Random.Range(0f, 1f);
        
        if (randomValue < wave.mixChance)
        {
            return GetRandomZombieIndex(wave);
        }
        else
        {
            return GetSequentialZombieIndex(wave);
        }
    }
    
    private int GetRandomZombieIndex(WaveData wave)
    {
        int totalRemaining = 0;
        for (int i = 0; i < wave.zombies.Length; i++)
        {
            if (wave.zombies[i] != null && wave.zombies[i].zombiePrefab != null)
            {
                totalRemaining += remainingZombies[i];
            }
        }
        
        if (totalRemaining <= 0)
        {
            return -1;
        }
        
        int randomPos = Random.Range(0, totalRemaining);
        int currentPos = 0;
        
        for (int i = 0; i < wave.zombies.Length; i++)
        {
            if (wave.zombies[i] != null && wave.zombies[i].zombiePrefab != null)
            {
                currentPos += remainingZombies[i];
                if (randomPos < currentPos)
                {
                    return i;
                }
            }
        }
        
        return -1;
    }
    
    private int GetSequentialZombieIndex(WaveData wave)
    {
        while (currentZombieTypeIndex < wave.zombies.Length)
        {
            WaveZombie currentZombie = wave.zombies[currentZombieTypeIndex];
            if (currentZombie != null && currentZombie.zombiePrefab != null)
            {
                if (remainingZombies[currentZombieTypeIndex] > 0)
                {
                    return currentZombieTypeIndex;
                }
                else
                {
                    currentZombieTypeIndex++;
                }
            }
            else
            {
                currentZombieTypeIndex++;
            }
        }
        
        return -1;
    }
    
    private void UpdateWaveText()
    {
        if (waveText != null && waves != null)
        {
            waveText.text = $"第 {currentWave + 1} / {waves.Length} 波";
        }
    }
    
    public void OnWaveComplete()
    {
        currentWave++;
        
        if (currentWave >= waves.Length)
        {
            return;
        }
        
        isWaitingForNextWave = true;
        waveDelayTimer = 0f;
    }
    
    public void ResetSpawner()
    {
        currentWave = 0;
        currentWaveSpawnCount = 0;
        totalWaveZombies = 0;
        currentZombieTypeIndex = 0;
        remainingZombies = null;
        isSpawning = false;
        isWaitingForNextWave = false;
        waveDelayTimer = 0f;
        timer = 0;
        UpdateWaveText();
        StartWave();
    }
}
