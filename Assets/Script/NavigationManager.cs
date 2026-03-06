using UnityEngine;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    [Header("导航按钮")]
    public Button homeButton;
    public Button profileButton;
    public Button shopButton;
    public Button settingsButton;

    [Header("页面")]
    public GameObject homePage;
    public GameObject profilePage;
    public GameObject shopPage;
    public GameObject settingsPage;

    [Header("选中状态")]
    public Color selectedColor = new Color(0.2f, 0.6f, 1f);
    public Color normalColor = new Color(0.4f, 0.4f, 0.4f);

    private void Start()
    {
        // 注册按钮点击事件
        homeButton.onClick.AddListener(() => ShowPage(homePage));
        profileButton.onClick.AddListener(() => ShowPage(profilePage));
        shopButton.onClick.AddListener(() => ShowPage(shopPage));
        settingsButton.onClick.AddListener(() => ShowPage(settingsPage));

        // 默认显示首页
        ShowPage(homePage);
    }

    /// <summary>
    /// 显示指定页面
    /// </summary>
    public void ShowPage(GameObject targetPage)
    {
        // 隐藏所有页面
        homePage.SetActive(false);
        profilePage.SetActive(false);
        shopPage.SetActive(false);
        settingsPage.SetActive(false);

        // 显示目标页面
        if (targetPage != null)
        {
            targetPage.SetActive(true);
        }

        // 更新按钮选中状态
        UpdateButtonStates(targetPage);
    }

    /// <summary>
    /// 更新按钮选中状态
    /// </summary>
    private void UpdateButtonStates(GameObject activePage)
    {
        // 重置所有按钮状态
        SetButtonState(homeButton, activePage == homePage);
        SetButtonState(profileButton, activePage == profilePage);
        SetButtonState(shopButton, activePage == shopPage);
        SetButtonState(settingsButton, activePage == settingsPage);
    }

    /// <summary>
    /// 设置按钮状态
    /// </summary>
    private void SetButtonState(Button button, bool isSelected)
    {
        if (button == null) return;

        // 获取按钮的Image组件
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isSelected ? selectedColor : normalColor;
        }
    }
}