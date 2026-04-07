using UnityEngine;
using UnityEngine.UI;

public class UpdateNotesPanel : MonoBehaviour
{
    [Header("UI元素")]
    public GameObject panel;
    public Button openButton;
    public Button closeButton;
    public ScrollRect scrollRect;
    public Text versionText;
    public Text contentText;
    
    [Header("更新内容配置")]
    [TextArea(5, 20)]
    public string updateContent = 
        "【版本更新内容】\n\n" +
        "1. 新增波次提前开始功能\n" +
        "   - 点击可提前开始下一波\n" +
        "   - 提前点击可获得金币奖励\n\n" +
        "2. 优化游戏体验\n" +
        "   - 改进金币动画效果\n" +
        "   - 修复多个已知问题\n\n" +
        "3. UI优化\n" +
        "   - 新增更新简报功能\n" +
        "   - 优化按钮点击反馈";
    
    public string versionNumber = "v1.0.0";
    
    private bool isInitialized = false;
    
    private void Awake()
    {
        DoInit();
    }
    
    private void DoInit()
    {
        if (isInitialized) return;
        isInitialized = true;
        
        panel.SetActive(false);
        
        if (openButton != null)
        {
            openButton.onClick.RemoveAllListeners();
            openButton.onClick.AddListener(ShowPanel);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HidePanel);
        }
        
        UpdateContent();
    }
    
    private void UpdateContent()
    {
        if (versionText != null)
            versionText.text = "版本 " + versionNumber;
        
        if (contentText != null)
            contentText.text = updateContent;
    }
    
    public void ShowPanel()
    {
        if (!isInitialized) DoInit();
        panel.SetActive(true);
        
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
    
    public void HidePanel()
    {
        panel.SetActive(false);
    }
    
    public void OnClickOutside()
    {
        HidePanel();
    }
    
    public void SetUpdateContent(string version, string content)
    {
        versionNumber = version;
        updateContent = content;
        UpdateContent();
    }
}
