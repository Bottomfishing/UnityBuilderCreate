using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("UI组件")]
    public Text messageText;
    public Text stepText;
    public Button nextButton;
    public Button skipButton;
    public GameObject background;
    public CanvasGroup canvasGroup;
    
    [Header("动画设置")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.2f;
    
    private bool hasAddedListeners = false;
    
    private void Awake()
    {
        if (background == null)
        {
            background = gameObject;
        }
        
        Image bgImage = background.GetComponent<Image>();
        if (bgImage == null)
        {
            bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.7f);
        }
        bgImage.raycastTarget = true;
        
        AutoFindComponents();
        
        AddButtonListeners();
    }
    
    private void AutoFindComponents()
    {
        if (messageText == null)
        {
            Text[] texts = GetComponentsInChildren<Text>();
            foreach (Text t in texts)
            {
                if (t.gameObject.name == "MessageText" || t.gameObject.name.ToLower().Contains("message"))
                {
                    messageText = t;
                    break;
                }
            }
        }
        
        if (stepText == null)
        {
            Text[] texts = GetComponentsInChildren<Text>();
            foreach (Text t in texts)
            {
                if (t.gameObject.name == "StepText" || t.gameObject.name.ToLower().Contains("step"))
                {
                    stepText = t;
                    break;
                }
            }
        }
        
        if (nextButton == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.gameObject.name == "NextButton" || btn.gameObject.name.ToLower().Contains("next"))
                {
                    nextButton = btn;
                    break;
                }
            }
        }
        
        if (skipButton == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.gameObject.name == "SkipButton" || btn.gameObject.name.ToLower().Contains("skip"))
                {
                    skipButton = btn;
                    break;
                }
            }
        }
        
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
    
    private void AddButtonListeners()
    {
        if (hasAddedListeners) return;
        
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnNextClick);
        }
        else
        {
            Debug.LogError("[TutorialUI] nextButton is NULL!");
        }
        
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(OnSkipClick);
        }
        else
        {
            Debug.LogError("[TutorialUI] skipButton is NULL!");
        }
        
        hasAddedListeners = true;
    }
    
    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.LogError("[TutorialUI] Canvas Group is NULL!");
        }
    }
    
    public void ShowMessage(string message, int currentStep, int totalSteps)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
        else
        {
            Debug.LogError("[TutorialUI] messageText is NULL!");
        }
        
        if (stepText != null)
        {
            stepText.text = $"{currentStep} / {totalSteps}";
        }
        else
        {
            Debug.LogError("[TutorialUI] stepText is NULL!");
        }
        
        if (nextButton != null)
        {
            Text btnText = nextButton.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = currentStep >= totalSteps ? "开始游戏" : "下一步";
            }
        }
    }
    
    private void OnNextClick()
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.NextStep();
        }
    }
    
    private void OnSkipClick()
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.SkipTutorial();
        }
    }
    
    private System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
    
    public System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
