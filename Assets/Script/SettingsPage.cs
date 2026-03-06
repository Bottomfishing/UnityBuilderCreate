using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour
{
    [Header("音量滑块")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("音量文本")]
    public Text masterVolumeText;
    public Text musicVolumeText;
    public Text sfxVolumeText;

    private void Start()
    {
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        if (AudioManager.instance == null)
        {
            return;
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = AudioManager.instance.GetMasterVolume();
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            UpdateMasterVolumeText(masterVolumeSlider.value);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = AudioManager.instance.GetMusicVolume();
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            UpdateMusicVolumeText(musicVolumeSlider.value);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = AudioManager.instance.GetSFXVolume();
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            UpdateSFXVolumeText(sfxVolumeSlider.value);
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMasterVolume(value);
        }
        UpdateMasterVolumeText(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(value);
        }
        UpdateMusicVolumeText(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSFXVolume(value);
        }
        UpdateSFXVolumeText(value);
    }

    private void UpdateMasterVolumeText(float value)
    {
        if (masterVolumeText != null)
        {
            masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }

    private void UpdateMusicVolumeText(float value)
    {
        if (musicVolumeText != null)
        {
            musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }

    private void UpdateSFXVolumeText(float value)
    {
        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }
}
