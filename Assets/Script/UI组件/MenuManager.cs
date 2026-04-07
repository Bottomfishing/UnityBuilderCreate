using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI面板")]
    public EnergyNotEnoughPanel energyNotEnoughPanel;
    
    private void Start()
    {
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnEnergyNotEnough -= ShowEnergyNotEnoughPanel;
            ResourceManager.instance.OnEnergyNotEnough += ShowEnergyNotEnoughPanel;
        }
    }
    
    private void OnEnable()
    {
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnEnergyNotEnough -= ShowEnergyNotEnoughPanel;
            ResourceManager.instance.OnEnergyNotEnough += ShowEnergyNotEnoughPanel;
        }
    }
    
    private void OnDisable()
    {
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnEnergyNotEnough -= ShowEnergyNotEnoughPanel;
        }
    }
    
    private void OnDestroy()
    {
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnEnergyNotEnough -= ShowEnergyNotEnoughPanel;
        }
    }
    
    private void ShowEnergyNotEnoughPanel()
    {
        if (energyNotEnoughPanel != null)
        {
            energyNotEnoughPanel.ShowPanel();
        }
        else
        {
            Debug.LogWarning("MenuManager: EnergyNotEnoughPanel is not assigned!");
        }
    }
}
