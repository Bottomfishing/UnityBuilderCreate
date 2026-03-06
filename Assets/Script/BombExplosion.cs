using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    [Header("爆炸设置")]
    public float explosionDuration = 1f;
    public float maxScale = 3f;
    
    [Header("粒子效果")]
    public ParticleSystem explosionParticles;
    
    [Header("视觉效果")]
    public SpriteRenderer explosionSprite;
    public Color startColor = Color.yellow;
    public Color endColor = Color.red;
    
    private float timer;
    
    private void Start()
    {
        timer = explosionDuration;
        
        if (explosionParticles != null)
        {
            explosionParticles.Play();
        }
        
        if (explosionSprite != null)
        {
            explosionSprite.color = startColor;
        }
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
        
        float progress = 1 - (timer / explosionDuration);
        
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
