using UnityEngine;
using System.Collections.Generic;

public class TowerUnlockManager : MonoBehaviour
{
    private static TowerUnlockManager _instance;
    public static TowerUnlockManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject managerObj = new GameObject("TowerUnlockManager");
                _instance = managerObj.AddComponent<TowerUnlockManager>();
                DontDestroyOnLoad(managerObj);
                _instance.LoadUnlockedTowers();
            }
            return _instance;
        }
    }

    private List<string> unlockedTowers = new List<string>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            if (transform.parent != null)
            {
                transform.SetParent(null, false);
            }
            DontDestroyOnLoad(gameObject);
            LoadUnlockedTowers();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void InitializeDefaultTowers(List<TowerData> allTowers)
    {
        bool needsSave = false;

        foreach (TowerData tower in allTowers)
        {
            if (tower != null && tower.defaultUnlocked)
            {
                if (!unlockedTowers.Contains(tower.towerName))
                {
                    unlockedTowers.Add(tower.towerName);
                    needsSave = true;
                    Debug.Log("默认解锁炮塔: " + tower.towerName);
                }
            }
        }

        if (needsSave)
        {
            SaveUnlockedTowers();
        }
    }

    public void RegisterAndCheckTower(TowerData tower)
    {
        if (tower == null) return;

        if (tower.defaultUnlocked && !unlockedTowers.Contains(tower.towerName))
        {
            unlockedTowers.Add(tower.towerName);
            SaveUnlockedTowers();
        }
    }

    public bool IsTowerUnlocked(string towerName)
    {
        if (string.IsNullOrEmpty(towerName))
        {
            return false;
        }
        
        string trimmedName = towerName.Trim();
        return unlockedTowers.Contains(trimmedName);
    }

    public void UnlockTower(string towerName)
    {
        if (string.IsNullOrEmpty(towerName))
        {
            return;
        }
        
        string trimmedName = towerName.Trim();
        if (!unlockedTowers.Contains(trimmedName))
        {
            unlockedTowers.Add(trimmedName);
            SaveUnlockedTowers();
        }
    }

    private void LoadUnlockedTowers()
    {
        string saved = PlayerPrefs.GetString("UnlockedTowers", "");
        if (!string.IsNullOrEmpty(saved))
        {
            string[] names = saved.Split(',');
            foreach (string name in names)
            {
                string trimmedName = name.Trim();
                if (!string.IsNullOrEmpty(trimmedName))
                {
                    unlockedTowers.Add(trimmedName);
                }
            }
        }
    }

    private void SaveUnlockedTowers()
    {
        string saved = string.Join(",", unlockedTowers);
        PlayerPrefs.SetString("UnlockedTowers", saved);
        PlayerPrefs.Save();
    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteKey("UnlockedTowers");
        PlayerPrefs.Save();
        unlockedTowers.Clear();
    }
}
