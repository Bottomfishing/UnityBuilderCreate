using UnityEngine;
using UnityEngine.UI;

public class TutorialPointer : MonoBehaviour
{
    [Header("动画设置")]
    public float bounceHeight = 30f;
    public float bounceSpeed = 2f;
    public float rotateSpeed = 0f;
    
    [Header("视觉效果")]
    public Image pointerImage;
    public Color highlightColor = Color.yellow;
    
    private Vector2 originalAnchoredPosition;
    private float timeElapsed;
    private RectTransform rectTransform;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        if (rectTransform != null)
        {
            originalAnchoredPosition = rectTransform.anchoredPosition;
        }
    }
    
    private void Update()
    {
        timeElapsed += Time.unscaledDeltaTime;
        
        if (rectTransform != null)
        {
            float bounce = Mathf.Sin(timeElapsed * bounceSpeed) * bounceHeight;
            rectTransform.anchoredPosition = originalAnchoredPosition + Vector2.up * bounce;
        }
        
        if (rotateSpeed > 0)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.unscaledDeltaTime);
        }
    }
    
    public void SetAnchoredPosition(Vector2 position)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
            originalAnchoredPosition = position;
        }
    }
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
