using UnityEngine;
using System.Collections.Generic;

public class SplashHitEffect : MonoBehaviour
{
    [Header("粒子设置")]
    public int particleCount = 8;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float minScale = 0.15f;
    public float maxScale = 0.3f;
    public float lifetime = 0.5f;
    
    [Header("重力设置")]
    public float gravity = 9.8f;
    
    [Header("淡出设置")]
    public bool enableFade = true;
    public float fadeStartTime = 0.2f;
    
    [Header("颜色设置")]
    public bool useRandomColor = true;
    public Color baseColor = Color.white;
    public Color colorVariationMin = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color colorVariationMax = new Color(1f, 1f, 1f, 1f);
    
    [Header("参考Sprite")]
    public Sprite particleSprite;
    
    private List<GameObject> particles = new List<GameObject>();
    
    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        CreateParticles();
    }
    
    private void CreateParticles()
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = new GameObject("SplashParticle");
            particle.transform.SetParent(transform);
            particle.transform.localPosition = Vector3.zero;
            
            SpriteRenderer spriteRenderer = particle.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = particleSprite;
            spriteRenderer.sortingOrder = 100;
            
            Color particleColor = GetParticleColor();
            spriteRenderer.color = particleColor;
            
            float scale = Random.Range(minScale, maxScale);
            particle.transform.localScale = Vector3.one * scale;
            
            float angle = Random.Range(0f, 360f);
            float speed = Random.Range(minSpeed, maxSpeed);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            
            SplashParticle particleScript = particle.AddComponent<SplashParticle>();
            particleScript.Initialize(direction * speed, gravity, lifetime, enableFade, fadeStartTime, particleColor);
            
            particles.Add(particle);
        }
        
        Destroy(gameObject, lifetime + 0.1f);
    }
    
    private Color GetParticleColor()
    {
        if (useRandomColor)
        {
            float r = Random.Range(colorVariationMin.r, colorVariationMax.r);
            float g = Random.Range(colorVariationMin.g, colorVariationMax.g);
            float b = Random.Range(colorVariationMin.b, colorVariationMax.b);
            float a = Random.Range(colorVariationMin.a, colorVariationMax.a);
            return new Color(r, g, b, a);
        }
        else
        {
            return baseColor;
        }
    }
}

public class SplashParticle : MonoBehaviour
{
    private Vector2 velocity;
    private float gravity;
    private float lifetime;
    private float timer;
    private bool enableFade;
    private float fadeStartTime;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    public void Initialize(Vector2 initialVelocity, float gravityForce, float life, bool fade, float fadeStart, Color color)
    {
        velocity = initialVelocity;
        gravity = gravityForce;
        lifetime = life;
        enableFade = fade;
        fadeStartTime = fadeStart;
        originalColor = color;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        velocity.y -= gravity * Time.deltaTime;
        transform.position += (Vector3)velocity * Time.deltaTime;
        
        if (enableFade && spriteRenderer != null && timer >= fadeStartTime)
        {
            float fadeProgress = (timer - fadeStartTime) / (lifetime - fadeStartTime);
            Color color = originalColor;
            color.a = Mathf.Lerp(originalColor.a, 0f, fadeProgress);
            spriteRenderer.color = color;
        }
        
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
