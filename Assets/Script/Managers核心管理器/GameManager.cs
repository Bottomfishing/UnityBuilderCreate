using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("金钱设置")]
    public int currentMoney;
    
    [Header("生命值设置")]
    public int currentLives;
    public int lowLivesThreshold = 3;
    
    [Header("UI设置")]
    public Text moneyText;
    public Image moneyIcon;
    public GameObject moneyPanel;
    public Text livesText;
    public Image livesIcon;
    public GameObject livesPanel;
    
    [Header("动画设置")]
    public float damageAnimationDuration = 0.5f;
    public float moneyAnimationDuration = 0.5f;
    public float moneyJumpHeight = 20f;
    public Color normalColor = Color.white;
    public Color lowLivesColor = Color.red;
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeFromLevelData();
        UpdateMoneyText();
        UpdateLivesText();
    }
    
    private void InitializeFromLevelData()
    {
        if (LevelDataContainer.selectedLevelData != null)
        {
            currentMoney = LevelDataContainer.selectedLevelData.startingMoney;
            currentLives = LevelDataContainer.selectedLevelData.startingLives;
        }
        else
        {
            currentMoney = 1000;
            currentLives = 10;
        }
    }
    
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyText();
        
        if (amount > 0)
        {
            StartCoroutine(PlayMoneyAnimation());
        }
    }
    
    private IEnumerator PlayMoneyAnimation()
    {
        if (moneyPanel != null)
        {
            Vector3 originalPosition = moneyPanel.transform.localPosition;
            Vector3 jumpPosition = originalPosition + new Vector3(0, moneyJumpHeight, 0);
            
            float elapsedTime = 0f;
            while (elapsedTime < moneyAnimationDuration / 2 && moneyPanel != null)
            {
                moneyPanel.transform.localPosition = Vector3.Lerp(originalPosition, jumpPosition, elapsedTime / (moneyAnimationDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            if (moneyPanel == null) yield break;
            
            elapsedTime = 0f;
            while (elapsedTime < moneyAnimationDuration / 2 && moneyPanel != null)
            {
                moneyPanel.transform.localPosition = Vector3.Lerp(jumpPosition, originalPosition, elapsedTime / (moneyAnimationDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            if (moneyPanel != null)
            {
                moneyPanel.transform.localPosition = originalPosition;
            }
        }
    }
    
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyText();
            return true;
        }
        else
        {
            StartCoroutine(PlayNoMoneyAnimation());
            return false;
        }
    }
    
    private IEnumerator PlayNoMoneyAnimation()
    {
        if (moneyPanel == null && moneyIcon == null) yield break;
        
        Image targetImage = null;
        Color originalColor = Color.white;
        
        if (moneyIcon != null)
        {
            targetImage = moneyIcon;
            originalColor = moneyIcon.color;
        }
        else if (moneyPanel != null)
        {
            targetImage = moneyPanel.GetComponent<Image>();
            if (targetImage != null)
            {
                originalColor = targetImage.color;
            }
        }
        
        if (targetImage == null) yield break;
        
        Color flashColor = new Color(1f, 0.2f, 0.2f);
        float flashDuration = 0.15f;
        int flashCount = 3;
        
        for (int i = 0; i < flashCount; i++)
        {
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                targetImage.color = Color.Lerp(originalColor, flashColor, elapsed / flashDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            elapsed = 0f;
            while (elapsed < flashDuration)
            {
                targetImage.color = Color.Lerp(flashColor, originalColor, elapsed / flashDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        targetImage.color = originalColor;
    }
    
    public int GetCurrentMoney()
    {
        return currentMoney;
    }
    
    public void LoseLife(int amount = 1)
    {
        int oldLives = currentLives;
        currentLives = Mathf.Max(0, currentLives - amount);
        
        if (currentLives < oldLives)
        {
            StartCoroutine(PlayDamageAnimation());
        }
        
        UpdateLivesText();
        
        if (currentLives <= 0)
        {
            GameOver();
        }
    }
    
    private IEnumerator PlayDamageAnimation()
    {
        if (livesPanel != null)
        {
            Color originalColor = livesPanel.GetComponent<Image>().color;
            
            for (int i = 0; i < 2; i++)
            {
                livesPanel.GetComponent<Image>().color = lowLivesColor;
                yield return new WaitForSeconds(damageAnimationDuration / 4);
                livesPanel.GetComponent<Image>().color = originalColor;
                yield return new WaitForSeconds(damageAnimationDuration / 4);
            }
        }
        
        if (livesText != null)
        {
            Color originalTextColor = livesText.color;
            livesText.color = lowLivesColor;
            yield return new WaitForSeconds(damageAnimationDuration);
            livesText.color = originalTextColor;
        }
    }
    
    private IEnumerator PlayLowLivesWarning()
    {
        if (livesText != null)
        {
            while (currentLives > 0 && currentLives <= lowLivesThreshold)
            {
                livesText.color = lowLivesColor;
                yield return new WaitForSeconds(0.5f);
                livesText.color = normalColor;
                yield return new WaitForSeconds(0.5f);
            }
            
            if (livesText != null)
            {
                livesText.color = normalColor;
            }
        }
    }
    
    private void GameOver()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.LevelLose();
        }
    }
    
    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = "金钱: " + currentMoney.ToString();
        }
    }
    
    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "生命: " + currentLives.ToString();
            
            if (currentLives > 0 && currentLives <= lowLivesThreshold)
            {
                StopCoroutine(PlayLowLivesWarning());
                StartCoroutine(PlayLowLivesWarning());
            }
            else
            {
                livesText.color = normalColor;
                StopCoroutine(PlayLowLivesWarning());
            }
        }
    }
    
    public int GetCurrentLives()
    {
        return currentLives;
    }
    
    public void ResetGameManager()
    {
        InitializeFromLevelData();
        UpdateMoneyText();
        UpdateLivesText();
    }
}
