using UnityEngine;
using UnityEngine.UI;

public class UIClickEffect : MonoBehaviour
{
    [Header("点击环设置")]
    public int ringCount = 3;
    public float ringMaxScale = 2f;
    public float ringLifetime = 0.4f;
    
    [Header("粒子设置")]
    public int particleCount = 8;
    public float particleSpeed = 100f;
    public float particleMinScale = 0.1f;
    public float particleMaxScale = 0.2f;
    public float particleLifetime = 0.5f;
    
    [Header("颜色设置")]
    public Color effectColor = new Color(1f, 1f, 1f, 0.8f);
    
    [Header("参考Sprite")]
    public Sprite ringSprite;
    public Sprite particleSprite;
    
    private RectTransform rectTransform;
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        CreateRings();
        CreateParticles();
        StartCoroutine(DestroyAfterDelay(Mathf.Max(ringLifetime, particleLifetime) + 0.1f));
    }
    
    private System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
    
    private void CreateRings()
    {
        for (int i = 0; i < ringCount; i++)
        {
            GameObject ring = new GameObject("ClickRing");
            ring.transform.SetParent(transform, false);
            
            Image image = ring.AddComponent<Image>();
            image.sprite = ringSprite;
            image.color = effectColor;
            image.raycastTarget = false;
            
            RectTransform ringRect = ring.GetComponent<RectTransform>();
            ringRect.sizeDelta = new Vector2(100f, 100f);
            ringRect.anchorMin = new Vector2(0.5f, 0.5f);
            ringRect.anchorMax = new Vector2(0.5f, 0.5f);
            ringRect.pivot = new Vector2(0.5f, 0.5f);
            
            UIClickRing ringScript = ring.AddComponent<UIClickRing>();
            float startScale = 0.5f + i * 0.3f;
            float delay = i * 0.08f;
            ringScript.Initialize(startScale, ringMaxScale, ringLifetime, delay, effectColor);
        }
    }
    
    private void CreateParticles()
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = new GameObject("ClickParticle");
            particle.transform.SetParent(transform, false);
            
            Image image = particle.AddComponent<Image>();
            image.sprite = particleSprite;
            image.color = effectColor;
            image.raycastTarget = false;
            
            RectTransform particleRect = particle.GetComponent<RectTransform>();
            float scale = Random.Range(particleMinScale, particleMaxScale);
            particleRect.sizeDelta = new Vector2(50f * scale, 50f * scale);
            particleRect.anchorMin = new Vector2(0.5f, 0.5f);
            particleRect.anchorMax = new Vector2(0.5f, 0.5f);
            particleRect.pivot = new Vector2(0.5f, 0.5f);
            
            float angle = Random.Range(0f, 360f);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            
            UIClickParticle particleScript = particle.AddComponent<UIClickParticle>();
            particleScript.Initialize(direction * particleSpeed, particleLifetime, effectColor);
        }
    }
}

public class UIClickRing : MonoBehaviour
{
    private float startScale;
    private float maxScale;
    private float lifetime;
    private float delay;
    private Color color;
    private float timer;
    private Image image;
    private RectTransform rectTransform;
    
    public void Initialize(float start, float max, float life, float delayTime, Color ringColor)
    {
        startScale = start;
        maxScale = max;
        lifetime = life;
        delay = delayTime;
        color = ringColor;
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one * startScale;
    }
    
    private void Update()
    {
        if (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            return;
        }
        
        float lifeTimer = timer - delay;
        float progress = lifeTimer / lifetime;
        
        float scale = Mathf.Lerp(startScale, maxScale, progress);
        rectTransform.localScale = Vector3.one * scale;
        
        if (image != null)
        {
            Color newColor = color;
            newColor.a = Mathf.Lerp(color.a, 0f, progress);
            image.color = newColor;
        }
        
        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
        }
        
        timer += Time.unscaledDeltaTime;
    }
}

public class UIClickParticle : MonoBehaviour
{
    private Vector2 velocity;
    private float lifetime;
    private float timer;
    private Color color;
    private Image image;
    private RectTransform rectTransform;
    
    public void Initialize(Vector2 initialVelocity, float life, Color particleColor)
    {
        velocity = initialVelocity;
        lifetime = life;
        color = particleColor;
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        
        rectTransform.anchoredPosition += velocity * Time.unscaledDeltaTime;
        velocity *= 0.95f;
        
        if (image != null)
        {
            float progress = timer / lifetime;
            Color newColor = color;
            newColor.a = Mathf.Lerp(color.a, 0f, progress);
            image.color = newColor;
        }
        
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
