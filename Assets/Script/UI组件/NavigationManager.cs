using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NavButtonConfig
{
    public Button button;
    public GameObject page;
    public Transform icon;
    public Transform text;
    public Vector3 iconNormalScale = Vector3.one;
    public Vector3 iconSelectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 textNormalScale = Vector3.one;
    public Vector3 textSelectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 iconNormalPosition = Vector3.zero;
    public Vector3 iconSelectedPosition = Vector3.zero;
    public bool useIconPositionChange = false;
}

public class NavigationManager : MonoBehaviour
{
    [Header("导航配置")]
    public NavButtonConfig[] navButtons;

    [Header("全局选中状态")]
    public Color selectedColor = new Color(0.2f, 0.6f, 1f);
    public Color normalColor = new Color(0.4f, 0.4f, 0.4f);

    private void Start()
    {
        if (navButtons == null || navButtons.Length == 0)
        {
            return;
        }

        for (int i = 0; i < navButtons.Length; i++)
        {
            int index = i;
            if (navButtons[index].button != null)
            {
                navButtons[index].button.onClick.AddListener(() => ShowPage(navButtons[index].page));
            }
        }

        SaveInitialStates();

        if (navButtons.Length > 0 && navButtons[0].page != null)
        {
            ShowPage(navButtons[0].page);
        }
    }

    private void SaveInitialStates()
    {
        for (int i = 0; i < navButtons.Length; i++)
        {
            if (navButtons[i].icon != null)
            {
                if (navButtons[i].iconNormalScale == Vector3.one)
                {
                    navButtons[i].iconNormalScale = navButtons[i].icon.localScale;
                }
                if (navButtons[i].iconNormalPosition == Vector3.zero)
                {
                    navButtons[i].iconNormalPosition = navButtons[i].icon.localPosition;
                }
            }

            if (navButtons[i].text != null)
            {
                if (navButtons[i].textNormalScale == Vector3.one)
                {
                    navButtons[i].textNormalScale = navButtons[i].text.localScale;
                }
            }
        }
    }

    public void ShowPage(GameObject targetPage)
    {
        for (int i = 0; i < navButtons.Length; i++)
        {
            if (navButtons[i].page != null)
            {
                navButtons[i].page.SetActive(navButtons[i].page == targetPage);
            }
        }

        UpdateButtonStates(targetPage);
    }

    private void UpdateButtonStates(GameObject activePage)
    {
        for (int i = 0; i < navButtons.Length; i++)
        {
            bool isSelected = navButtons[i].page == activePage;
            SetButtonState(navButtons[i], isSelected);
        }
    }

    private void SetButtonState(NavButtonConfig config, bool isSelected)
    {
        if (config.button == null) return;

        Image buttonImage = config.button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isSelected ? selectedColor : normalColor;
        }

        if (config.icon != null)
        {
            config.icon.localScale = isSelected ? config.iconSelectedScale : config.iconNormalScale;
            
            if (config.useIconPositionChange)
            {
                config.icon.localPosition = isSelected ? config.iconSelectedPosition : config.iconNormalPosition;
            }
        }

        if (config.text != null)
        {
            config.text.localScale = isSelected ? config.textSelectedScale : config.textNormalScale;
        }
    }
}