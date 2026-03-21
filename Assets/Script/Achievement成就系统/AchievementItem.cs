using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;
    public Text nameText;
    public Text descriptionText;
    public Text progressText;
    public Image progressFill;
    public Image completedIcon;
    public GameObject completedEffect;

    [Header("视觉设置")]
    public Color normalColor = Color.white;
    public Color completedColor = Color.yellow;
    public Color progressColor = Color.green;

    public AchievementData achievementData { get; private set; }

    public void Setup(AchievementData data)
    {
        achievementData = data;

        if (nameText != null)
        {
            nameText.text = data.achievementName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = data.description;
        }

        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
        }

        UpdateVisual();
    }

    public void UpdateProgress(int currentValue, int targetValue)
    {
        if (achievementData == null) return;

        achievementData.currentValue = currentValue;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (achievementData == null) return;

        float progress = 0f;
        if (achievementData.targetValue > 0)
        {
            progress = Mathf.Clamp01((float)achievementData.currentValue / achievementData.targetValue);
        }

        if (progressText != null)
        {
            progressText.text = $"{achievementData.currentValue}/{achievementData.targetValue}";
        }

        if (progressFill != null)
        {
            progressFill.fillAmount = progress;
        }

        if (achievementData.isCompleted)
        {
            SetCompletedVisual();
        }
        else
        {
            SetNormalVisual();
        }
    }

    private void SetCompletedVisual()
    {
        if (completedIcon != null)
        {
            completedIcon.gameObject.SetActive(true);
        }

        if (completedEffect != null)
        {
            completedEffect.SetActive(true);
        }

        if (progressFill != null)
        {
            progressFill.color = completedColor;
        }
    }

    private void SetNormalVisual()
    {
        if (completedIcon != null)
        {
            completedIcon.gameObject.SetActive(false);
        }

        if (completedEffect != null)
        {
            completedEffect.SetActive(false);
        }

        if (progressFill != null)
        {
            progressFill.color = progressColor;
        }
    }
}
