using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    [Header("冰冻特效设置")]
    public SpriteRenderer freezeSprite;
    public float effectDuration = 2f;
    public float maxScale = 3f;
    public Color startColor = new Color(0f, 1f, 1f, 0.8f);
    public Color endColor = new Color(0f, 1f, 1f, 0f);
    
    private float timer;
    
    private void Start()
    {
        timer = effectDuration;
        
        if (freezeSprite != null)
        {
            freezeSprite.color = startColor;
            transform.localScale = Vector3.one * 0.1f;
        }
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
        float progress = 1 - (timer / effectDuration);
        
        if (freezeSprite != null)
        {
            float scale = Mathf.Lerp(0.1f, maxScale, progress);
            transform.localScale = Vector3.one * scale;
            freezeSprite.color = Color.Lerp(startColor, endColor, progress);
            Color color = freezeSprite.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            freezeSprite.color = color;
        }
        
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
