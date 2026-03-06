using UnityEngine;
using UnityEngine.UI;

public class LevelBackgroundManager : MonoBehaviour
{
    [Header("背景图片")]
    public Image backgroundImage;
    
    private void Start()
    {
        LoadLevelBackground();
    }
    
    private void LoadLevelBackground()
    {
        if (LevelDataContainer.selectedLevelData == null)
        {
            return;
        }
        
        if (backgroundImage != null && LevelDataContainer.selectedLevelData.backgroundImage != null)
        {
            backgroundImage.sprite = LevelDataContainer.selectedLevelData.backgroundImage;
        }
    }
}
