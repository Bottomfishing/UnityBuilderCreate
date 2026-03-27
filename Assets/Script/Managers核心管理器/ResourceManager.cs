using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    
    private PlayerData playerData;
    
    [Header("默认资源")]
    public int defaultCoins = 0;
    public int defaultDiamonds = 0;
    public int defaultEnergy = 100;
    public int defaultMaxEnergy = 100;
    
    [Header("体力恢复设置")]
    public bool enableEnergyRegen = true;
    public int energyRegenAmount = 1;
    public float energyRegenInterval = 60f;
    
    public delegate void ResourceChanged();
    public event ResourceChanged OnResourceChanged;
    
    public delegate void EnergyNotEnough();
    public event EnergyNotEnough OnEnergyNotEnough;
    
    private float energyRegenTimer = 0f;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null, false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        LoadData();
        CalculateOfflineEnergyRegen();
    }
    
    private void Update()
    {
        if (enableEnergyRegen)
        {
            RegenerateEnergy();
        }
        
        HandleDebugKeys();
    }
    
    private void HandleDebugKeys()
    {
        // 只在非游戏场景中使用资源调试快捷键
        if (LevelManager.instance != null)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddCoins(1000);
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            AddDiamonds(100);
        }
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            playerData.energy = playerData.maxEnergy;
            SaveData();
        }
        
        if (Input.GetKeyDown(KeyCode.F4))
        {
            AddCoins(10000);
            AddDiamonds(1000);
            playerData.energy = playerData.maxEnergy;
            SaveData();
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ResetData();
        }
    }
    
    public void LoadData()
    {
        string json = PlayerPrefs.GetString("PlayerData", "");
        
        if (!string.IsNullOrEmpty(json))
        {
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData = new PlayerData();
            playerData.coins = defaultCoins;
            playerData.diamonds = defaultDiamonds;
            playerData.energy = defaultEnergy;
            playerData.maxEnergy = defaultMaxEnergy;
            SaveData();
        }
        
        NotifyResourceChanged();
    }
    
    public void SaveData()
    {
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
        NotifyResourceChanged();
    }
    
    public int GetCoins()
    {
        return playerData.coins;
    }
    
    public int GetDiamonds()
    {
        return playerData.diamonds;
    }
    
    public int GetEnergy()
    {
        return playerData.energy;
    }
    
    public int GetMaxEnergy()
    {
        return playerData.maxEnergy;
    }
    
    public float GetEnergyRegenTimeRemaining()
    {
        if (playerData.energy >= playerData.maxEnergy)
        {
            return 0f;
        }
        
        return energyRegenInterval - energyRegenTimer;
    }
    
    public void AddCoins(int amount)
    {
        playerData.coins += amount;
        SaveData();
        NotifyResourceChanged();
    }

    public void AddDiamonds(int amount)
    {
        playerData.diamonds += amount;
        SaveData();
        NotifyResourceChanged();
    }

    public void AddEnergy(int amount)
    {
        playerData.energy = Mathf.Min(playerData.energy + amount, playerData.maxEnergy);
        SaveData();
        NotifyResourceChanged();
    }
    
    public bool SpendCoins(int amount)
    {
        if (playerData.coins >= amount)
        {
            playerData.coins -= amount;
            SaveData();
            return true;
        }
        return false;
    }
    
    public bool SpendDiamonds(int amount)
    {
        if (playerData.diamonds >= amount)
        {
            playerData.diamonds -= amount;
            SaveData();
            return true;
        }
        return false;
    }
    
    public bool SpendEnergy(int amount)
    {
        if (playerData.energy >= amount)
        {
            playerData.energy -= amount;
            SaveData();
            return true;
        }
        
        // 触发体力不足事件
        if (OnEnergyNotEnough != null)
        {
            OnEnergyNotEnough();
        }
        
        return false;
    }
    
    public void SetMaxEnergy(int maxEnergy)
    {
        playerData.maxEnergy = maxEnergy;
        if (playerData.energy > maxEnergy)
        {
            playerData.energy = maxEnergy;
        }
        SaveData();
    }
    
    public void ResetData()
    {
        playerData = new PlayerData();
        playerData.coins = defaultCoins;
        playerData.diamonds = defaultDiamonds;
        playerData.energy = defaultEnergy;
        playerData.maxEnergy = defaultMaxEnergy;
        SaveData();
    }
    
    private void NotifyResourceChanged()
    {
        if (OnResourceChanged != null)
        {
            OnResourceChanged();
        }
    }
    
    private void RegenerateEnergy()
    {
        if (playerData.energy >= playerData.maxEnergy)
        {
            return;
        }
        
        energyRegenTimer += Time.deltaTime;
        
        if (energyRegenTimer >= energyRegenInterval)
        {
            AddEnergy(energyRegenAmount);
            energyRegenTimer = 0f;
        }
    }
    
    private void CalculateOfflineEnergyRegen()
    {
        if (PlayerPrefs.HasKey("LastExitTime"))
        {
            string lastExitTimeString = PlayerPrefs.GetString("LastExitTime");
            DateTime lastExitTime = DateTime.Parse(lastExitTimeString);
            DateTime currentTime = DateTime.Now;
            
            TimeSpan timePassed = currentTime - lastExitTime;
            double totalSecondsPassed = timePassed.TotalSeconds;
            
            if (totalSecondsPassed > 0 && playerData.energy < playerData.maxEnergy)
            {
                int regenAmount = Mathf.FloorToInt((float)totalSecondsPassed / energyRegenInterval) * energyRegenAmount;
                if (regenAmount > 0)
                {
                    AddEnergy(regenAmount);
                }
            }
        }
        
        SaveExitTime();
    }
    
    private void SaveExitTime()
    {
        string currentTimeString = DateTime.Now.ToString("o");
        PlayerPrefs.SetString("LastExitTime", currentTimeString);
        PlayerPrefs.Save();
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveExitTime();
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveExitTime();
    }
}
