using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    [Header("UI元素")]
    public Text coinsText;
    public Text diamondsText;
    public Text energyText;
    public Text regenTimeText;
    
    private void Start()
    {
        UpdateUI();
        
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnResourceChanged += UpdateUI;
        }
    }
    
    private void OnDestroy()
    {
        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnResourceChanged -= UpdateUI;
        }
    }
    
    private void Update()
    {
        UpdateRegenTime();
    }
    
    public void UpdateUI()
    {
        if (ResourceManager.instance == null)
        {
            return;
        }
        
        if (coinsText != null)
        {
            coinsText.text = ResourceManager.instance.GetCoins().ToString();
        }
        
        if (diamondsText != null)
        {
            diamondsText.text = ResourceManager.instance.GetDiamonds().ToString();
        }
        
        if (energyText != null)
        {
            int energy = ResourceManager.instance.GetEnergy();
            int maxEnergy = ResourceManager.instance.GetMaxEnergy();
            energyText.text = $"{energy}/{maxEnergy}";
        }
    }
    
    private void UpdateRegenTime()
    {
        if (regenTimeText == null || ResourceManager.instance == null)
        {
            return;
        }
        
        float timeRemaining = ResourceManager.instance.GetEnergyRegenTimeRemaining();
        
        if (timeRemaining <= 0)
        {
            regenTimeText.text = "";
        }
        else
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            regenTimeText.text = $"{seconds}s后恢复1体力";
        }
    }
}
