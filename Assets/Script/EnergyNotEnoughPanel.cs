using UnityEngine;
using UnityEngine.UI;

public class EnergyNotEnoughPanel : MonoBehaviour
{
    [Header("UI元素")]
    public Button watchAdButton;
    public Button closeButton;
    public GameObject panel;
    
    private void Start()
    {
        // 初始化时隐藏面板
        panel.SetActive(false);
        
        // 添加按钮事件
        if (watchAdButton != null)
        {
            watchAdButton.onClick.AddListener(OnWatchAdButtonClick);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }
    }
    
    // 显示体力不足提示
    public void ShowPanel()
    {
        panel.SetActive(true);
    }
    
    // 隐藏体力不足提示
    public void HidePanel()
    {
        panel.SetActive(false);
    }
    
    // 看广告按钮点击事件
    private void OnWatchAdButtonClick()
    {
        // 这里可以添加广告逻辑
        Debug.Log("观看广告恢复体力！");
        
        // 恢复体力（示例）
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.AddEnergy(20);
        }
        
        HidePanel();
    }
    
    // 关闭按钮点击事件
    private void OnCloseButtonClick()
    {
        HidePanel();
    }
    
    // 点击框外关闭
    public void OnClickOutside()
    {
        HidePanel();
    }
}
