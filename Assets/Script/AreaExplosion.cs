using UnityEngine;

public class AreaExplosion : MonoBehaviour
{
    [Header("爆炸特效设置")]
    public SpriteRenderer explosionSprite;
    public float effectDuration = 0.5f;
    public float maxScale = 2f;
    public Color startColor = new Color(1f, 0.5f, 0f, 0.8f);
    public Color endColor = new Color(1f, 0.2f, 0f, 0f);
    
    private float timer;
    
    private void Start()
    {
        timer = effectDuration;
        
        if (explosionSprite != null)
        {
            explosionSprite.color = startColor;
            transform.localScale = Vector3.one * 0.1f;
        }
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
        float progress = 1 - (timer / effectDuration);
        
        if (explosionSprite != null)
        {
            float scale = Mathf.Lerp(0.1f, maxScale, progress);
            transform.localScale = Vector3.one * scale;
            explosionSprite.color = Color.Lerp(startColor, endColor, progress);
            Color color = explosionSprite.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            explosionSprite.color = color;
        }
        
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
