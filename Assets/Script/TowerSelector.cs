using UnityEngine;
using UnityEngine.UI;

public class TowerSelector : MonoBehaviour
{
    [Header("UI设置")]
    public Button towerButton; // 防御塔选择按钮
    public Image towerIcon; // 防御塔图标
    public Color selectedColor = Color.green; // 选中状态颜色
    public Color normalColor = Color.white; // 正常状态颜色
    public float selectedScale = 1.1f; // 选中状态缩放
    public float normalScale = 1f; // 正常状态缩放
    
    [Header("防御塔设置")]
    public GameObject towerPrefab; // 防御塔预制体
    
    private bool isTowerSelected = false; // 是否选中了防御塔
    
    private void Start()
    {
        // 为按钮添加点击事件
        if (towerButton != null)
        {
            towerButton.onClick.AddListener(ToggleTowerSelection);
        }
        
        // 初始化按钮状态
        UpdateButtonVisuals();
    }
    
    // 切换防御塔选择状态
    public void ToggleTowerSelection()
    {
        isTowerSelected = !isTowerSelected;
        UpdateButtonVisuals();
    }
    
    // 更新按钮视觉效果
    private void UpdateButtonVisuals()
    {
        if (towerIcon != null)
        {
            // 更改图标颜色
            towerIcon.color = isTowerSelected ? selectedColor : normalColor;
            
            // 更改按钮缩放
            towerButton.transform.localScale = Vector3.one * (isTowerSelected ? selectedScale : normalScale);
        }
    }
    
    // 获取当前选择状态
    public bool IsTowerSelected()
    {
        return isTowerSelected;
    }
    
    // 获取选中的防御塔预制体
    public GameObject GetSelectedTowerPrefab()
    {
        return isTowerSelected ? towerPrefab : null;
    }
}