using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("音频源")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("默认音量")]
    [Range(0f, 1f)]
    public float defaultMasterVolume = 1f;
    [Range(0f, 1f)]
    public float defaultMusicVolume = 1f;
    [Range(0f, 1f)]
    public float defaultSFXVolume = 1f;

    private float currentMasterVolume;
    private float currentMusicVolume;
    private float currentSFXVolume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            
            // 如果没有音频源，自动创建
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolumeSettings();
        Debug.Log("AudioManager 初始化完成");
    }

    public void SetMasterVolume(float volume)
    {
        currentMasterVolume = Mathf.Clamp01(volume);
        Debug.Log("设置主音量：" + currentMasterVolume);
        
        // 直接设置 AudioListener.volume
        AudioListener.volume = currentMasterVolume;
        
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        Debug.Log("设置音乐音量：" + currentMusicVolume);
        
        // 更新音乐源音量
        if (musicSource != null)
        {
            musicSource.volume = currentMusicVolume;
        }
        
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        currentSFXVolume = Mathf.Clamp01(volume);
        Debug.Log("设置音效音量：" + currentSFXVolume);
        
        // 更新音效源音量
        if (sfxSource != null)
        {
            sfxSource.volume = currentSFXVolume;
        }
        
        SaveVolumeSettings();
    }

    public float GetMasterVolume()
    {
        return currentMasterVolume;
    }

    public float GetMusicVolume()
    {
        return currentMusicVolume;
    }

    public float GetSFXVolume()
    {
        return currentSFXVolume;
    }

    // 播放音乐
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.volume = currentMusicVolume;
            musicSource.Play();
        }
    }

    // 停止音乐
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // 播放音效
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, currentSFXVolume);
        }
    }

    // 播放音效（指定音量）
    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, currentSFXVolume * volumeScale);
        }
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", currentMasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", currentSFXVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        currentMasterVolume = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        currentSFXVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);

        // 直接设置 AudioListener.volume
        AudioListener.volume = currentMasterVolume;
        
        // 更新音频源音量
        if (musicSource != null)
        {
            musicSource.volume = currentMusicVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = currentSFXVolume;
        }
    }
}
