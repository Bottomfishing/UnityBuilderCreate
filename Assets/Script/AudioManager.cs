using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("音频设置")]
    public AudioMixer audioMixer;
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolumeSettings();
    }

    public void SetMasterVolume(float volume)
    {
        currentMasterVolume = Mathf.Clamp01(volume);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(masterVolumeParam, VolumeToDecibel(currentMasterVolume));
        }
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(musicVolumeParam, VolumeToDecibel(currentMusicVolume));
        }
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        currentSFXVolume = Mathf.Clamp01(volume);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(sfxVolumeParam, VolumeToDecibel(currentSFXVolume));
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

    private float VolumeToDecibel(float volume)
    {
        return volume > 0 ? Mathf.Log10(volume) * 20f : -80f;
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

        SetMasterVolume(currentMasterVolume);
        SetMusicVolume(currentMusicVolume);
        SetSFXVolume(currentSFXVolume);
    }
}
