using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;
    public Text nameText;
    public Text descriptionText;
    public Button confirmButton;
    public GameObject effect;

    [Header("奖励显示")]
    public Image rewardCoinsIcon;
    public Text rewardCoinsText;
    public Image rewardDiamondsIcon;
    public Text rewardDiamondsText;
    public Image rewardEnergyIcon;
    public Text rewardEnergyText;
    public GameObject rewardPanel;

    [Header("动画设置")]
    public float showDuration = 3f;
    public float fadeSpeed = 1f;
    public Vector3 popupScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 normalScale = Vector3.one;

    private CanvasGroup canvasGroup;
    private float timer;
    private bool isInitialized = false;

    public static AchievementPopup instance { get; private set; }

    private void Awake()
    {
        DoInit();
    }
    
    private void DoInit()
    {
        if (isInitialized) return;
        isInitialized = true;
        
        instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(Hide);
        }
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        timer += Time.deltaTime;

        if (timer < showDuration * 0.3f)
        {
            float t = timer / (showDuration * 0.3f);
            transform.localScale = Vector3.Lerp(Vector3.zero, popupScale, t);
            canvasGroup.alpha = t;
        }
        else if (timer < showDuration * 0.7f)
        {
            transform.localScale = popupScale;
            canvasGroup.alpha = 1f;
        }
        else if (timer < showDuration)
        {
            float t = (timer - showDuration * 0.7f) / (showDuration * 0.3f);
            transform.localScale = Vector3.Lerp(popupScale, normalScale, t);
            canvasGroup.alpha = 1f - t;
        }
        else
        {
            Hide();
        }
    }

    public void ShowAchievement(AchievementData achievement)
    {
        if (achievement == null) return;
        
        if (!isInitialized) DoInit();

        gameObject.SetActive(true);
        timer = 0f;

        if (nameText != null)
        {
            nameText.text = achievement.achievementName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = achievement.description;
        }

        if (iconImage != null && achievement.icon != null)
        {
            iconImage.sprite = achievement.icon;
        }

        bool hasReward = false;

        if (rewardCoinsIcon != null && rewardCoinsText != null)
        {
            if (achievement.reward != null && achievement.reward.coins > 0)
            {
                if (achievement.reward.coinsIcon != null)
                {
                    rewardCoinsIcon.sprite = achievement.reward.coinsIcon;
                }
                rewardCoinsIcon.gameObject.SetActive(true);
                rewardCoinsText.text = $"+{achievement.reward.coins}";
                rewardCoinsText.gameObject.SetActive(true);
                hasReward = true;
            }
            else
            {
                rewardCoinsIcon.gameObject.SetActive(false);
                rewardCoinsText.gameObject.SetActive(false);
            }
        }

        if (rewardDiamondsIcon != null && rewardDiamondsText != null)
        {
            if (achievement.reward != null && achievement.reward.diamonds > 0)
            {
                if (achievement.reward.diamondsIcon != null)
                {
                    rewardDiamondsIcon.sprite = achievement.reward.diamondsIcon;
                }
                rewardDiamondsIcon.gameObject.SetActive(true);
                rewardDiamondsText.text = $"+{achievement.reward.diamonds}";
                rewardDiamondsText.gameObject.SetActive(true);
                hasReward = true;
            }
            else
            {
                rewardDiamondsIcon.gameObject.SetActive(false);
                rewardDiamondsText.gameObject.SetActive(false);
            }
        }

        if (rewardEnergyIcon != null && rewardEnergyText != null)
        {
            if (achievement.reward != null && achievement.reward.energy > 0)
            {
                if (achievement.reward.energyIcon != null)
                {
                    rewardEnergyIcon.sprite = achievement.reward.energyIcon;
                }
                rewardEnergyIcon.gameObject.SetActive(true);
                rewardEnergyText.text = $"+{achievement.reward.energy}";
                rewardEnergyText.gameObject.SetActive(true);
                hasReward = true;
            }
            else
            {
                rewardEnergyIcon.gameObject.SetActive(false);
                rewardEnergyText.gameObject.SetActive(false);
            }
        }

        if (rewardPanel != null)
        {
            rewardPanel.SetActive(hasReward);
        }

        if (effect != null)
        {
            effect.SetActive(true);
        }

        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        if (effect != null)
        {
            effect.SetActive(false);
        }
    }
}
