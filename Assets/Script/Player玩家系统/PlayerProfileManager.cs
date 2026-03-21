using UnityEngine;

public class PlayerProfileManager : MonoBehaviour
{
    public static PlayerProfileManager instance;

    [Header("默认设置")]
    public string defaultPlayerName = "玩家";
    public Sprite[] availableAvatars;

    private PlayerProfileData _profileData;

    public delegate void ProfileChanged();
    public event ProfileChanged OnProfileChanged;

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

        LoadProfile();
    }

    public string GetPlayerName()
    {
        return _profileData.playerName;
    }

    public Sprite GetAvatar()
    {
        if (availableAvatars != null && _profileData.avatarIndex >= 0 && _profileData.avatarIndex < availableAvatars.Length)
        {
            return availableAvatars[_profileData.avatarIndex];
        }
        return null;
    }

    public int GetAvatarIndex()
    {
        return _profileData.avatarIndex;
    }

    public Sprite[] GetAvailableAvatars()
    {
        return availableAvatars;
    }

    public void SetPlayerName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            _profileData.playerName = newName;
            SaveProfile();
            NotifyProfileChanged();
        }
    }

    public void SetAvatar(int index)
    {
        if (availableAvatars != null && index >= 0 && index < availableAvatars.Length)
        {
            _profileData.avatarIndex = index;
            SaveProfile();
            NotifyProfileChanged();
        }
    }

    public void ResetProfile()
    {
        _profileData = new PlayerProfileData();
        _profileData.playerName = defaultPlayerName;
        _profileData.avatarIndex = 0;
        SaveProfile();
        NotifyProfileChanged();
    }

    private void LoadProfile()
    {
        string json = PlayerPrefs.GetString("PlayerProfile", "");
        
        if (!string.IsNullOrEmpty(json))
        {
            _profileData = JsonUtility.FromJson<PlayerProfileData>(json);
        }
        else
        {
            _profileData = new PlayerProfileData();
            _profileData.playerName = defaultPlayerName;
            _profileData.avatarIndex = 0;
            SaveProfile();
        }
    }

    private void SaveProfile()
    {
        string json = JsonUtility.ToJson(_profileData);
        PlayerPrefs.SetString("PlayerProfile", json);
        PlayerPrefs.Save();
    }

    private void NotifyProfileChanged()
    {
        if (OnProfileChanged != null)
        {
            OnProfileChanged();
        }
    }
}

[System.Serializable]
public class PlayerProfileData
{
    public string playerName;
    public int avatarIndex;
}
