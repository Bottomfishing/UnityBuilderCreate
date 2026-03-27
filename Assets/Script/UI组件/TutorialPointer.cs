using UnityEngine;
using UnityEngine.UI;

public class TutorialPointer : MonoBehaviour
{
    [Header("动画设置")]
    public float bounceHeight = 0.3f;
    public float bounceSpeed = 2f;
    public float rotateSpeed = 0f;
    
    [Header("视觉效果")]
    public Image pointerImage;
    public Color highlightColor = Color.yellow;
    
    private Vector3 originalPosition;
    private float timeElapsed;
    
    private void Start()
    {
        originalPosition = transform.position;
    }
    
    private void Update()
    {
        timeElapsed += Time.unscaledDeltaTime;
        
        float bounce = Mathf.Sin(timeElapsed * bounceSpeed) * bounceHeight;
        transform.position = originalPosition + Vector3.up * bounce;
        
        if (rotateSpeed > 0)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.unscaledDeltaTime);
        }
    }
    
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        originalPosition = position;
    }
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
