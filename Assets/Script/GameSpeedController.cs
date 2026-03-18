using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    public static GameSpeedController instance;

    [Header("速度设置")]
    public float normalSpeed = 1f;
    public float fastSpeed = 2f;
    
    [Header("UI组件")]
    public Button speedButton;
    public Image buttonBorder;
    public Color normalColor = Color.white;
    public Color activeColor = Color.yellow;
    
    private bool isSpeedUp = false;
    
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
    }
    
    private void Start()
    {
        if (speedButton != null)
        {
            speedButton.onClick.AddListener(ToggleSpeed);
        }
        
        UpdateButtonVisual();
        Time.timeScale = normalSpeed;
    }
    
    public void ToggleSpeed()
    {
        isSpeedUp = !isSpeedUp;
        
        if (isSpeedUp)
        {
            Time.timeScale = fastSpeed;
        }
        else
        {
            Time.timeScale = normalSpeed;
        }
        
        UpdateButtonVisual();
    }
    
    private void UpdateButtonVisual()
    {
        if (buttonBorder != null)
        {
            buttonBorder.color = isSpeedUp ? activeColor : normalColor;
        }
    }
    
    public bool IsSpeedUp()
    {
        return isSpeedUp;
    }
    
    public float GetCurrentSpeed()
    {
        return isSpeedUp ? fastSpeed : normalSpeed;
    }
    
    private void OnDestroy()
    {
        Time.timeScale = normalSpeed;
    }
}
