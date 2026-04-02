using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnergyNotEnoughPanel : MonoBehaviour
{
    [Header("UI元素")]
    public Button watchAdButton;
    public Button closeButton;
    public GameObject panel;

    private bool isInitialized = false;

    private void Awake()
    {
        if (!isInitialized) DoInit();
    }

    private void DoInit()
    {
        if (isInitialized) return;
        isInitialized = true;

        panel.SetActive(false);

        if (watchAdButton != null)
        {
            watchAdButton.onClick.RemoveAllListeners();
            watchAdButton.onClick.AddListener(OnWatchAdButtonClick);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }
    }

    public void ShowPanel()
    {
        if (!isInitialized) DoInit();
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    private void OnWatchAdButtonClick()
    {
        if (ResourceManager.instance != null)
            ResourceManager.instance.AddEnergy(20);
        HidePanel();
    }

    private void OnCloseButtonClick()
    {
        HidePanel();
    }

    public void OnClickOutside()
    {
        HidePanel();
    }
}
