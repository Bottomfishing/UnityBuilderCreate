using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI面板")]
    public EnergyNotEnoughPanel energyNotEnoughPanel;
    
    private void Start()
    {
        // 订阅体力不足事件
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnEnergyNotEnough += ShowEnergyNotEnoughPanel;
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnEnergyNotEnough -= ShowEnergyNotEnoughPanel;
        }
    }
    
    // 显示体力不足提示
    private void ShowEnergyNotEnoughPanel()
    {
        if (energyNotEnoughPanel != null)
        {
            energyNotEnoughPanel.ShowPanel();
        }
    }
}
