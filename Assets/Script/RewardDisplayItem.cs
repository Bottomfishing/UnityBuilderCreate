using UnityEngine;
using UnityEngine.UI;

public class RewardDisplayItem : MonoBehaviour
{
    public Image iconImage;
    public Text amountText;
    
    public void Setup(RewardItem reward)
    {
        if (iconImage != null && reward.icon != null)
        {
            iconImage.sprite = reward.icon;
        }
        
        if (amountText != null)
        {
            amountText.text = "+" + reward.amount.ToString();
        }
    }
}
