using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("血量基础设置")]
    public int maxHealth = 5;
    public float healthBarYOffset = 0.8f;

    [Header("血条视觉设置")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float healthBarHeight = 0.2f;
    public float healthBarWidth = 1f;

    [Header("组件赋值")]
    public Transform healthBarRoot;
    public SpriteRenderer healthBarBG;
    public SpriteRenderer healthBarFill;

    [Header("简单受击效果")]
    public GameObject hitMarkPrefab;
    public AudioClip hitSound;
    private AudioSource audioSource;

    [Header("简单死亡效果")]
    public GameObject dieEffectPrefab;
    
    [Header("奖励设置")]
    public int reward = 50;

    [Header("分裂设置")]
    public bool canSplit = false;
    public GameObject splitZombiePrefab;
    public int splitCount = 2;
    public float splitOffset = 0.5f;

    [Header("护盾设置")]
    public bool useShield = false;
    public int shieldHits = 3;
    [Range(0f, 1f)]
    public float damageReductionPercent = 0.5f;
    public GameObject shieldEffectPrefab;
    public float shieldEffectDuration = 0.5f;
    public Color shieldColor = new Color(0f, 0.5f, 1f, 0.5f);
    public float shieldShowDuration = 1f;
    public float shieldScaleMultiplier = 1.2f;

    private int _currentHealth;
    private Transform _fillTransform;
    private bool isDead = false;
    private int remainingShieldHits;
    private GameObject currentShieldEffect;
    private SpriteRenderer zombieSpriteRenderer;

    private void Awake()
    {
        if (healthBarRoot == null) healthBarRoot = transform.Find("HealthBar");
        if (healthBarBG == null) healthBarBG = healthBarRoot?.Find("BG")?.GetComponent<SpriteRenderer>();
        if (healthBarFill == null) healthBarFill = healthBarRoot?.Find("Fill")?.GetComponent<SpriteRenderer>();
        _fillTransform = healthBarFill?.transform;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        _currentHealth = maxHealth;
        remainingShieldHits = shieldHits;
        InitHealthBarLayout();
        UpdateHealthBarVisual();
        
        zombieSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void InitHealthBarLayout()
    {
        if (healthBarRoot == null || healthBarBG == null || healthBarFill == null)
        {
            return;
        }

        healthBarRoot.localPosition = new Vector3(0, healthBarYOffset, 0);
        healthBarRoot.localScale = Vector3.one;
        healthBarRoot.localRotation = Quaternion.identity;

        healthBarBG.transform.localPosition = Vector3.zero;
        healthBarBG.transform.localScale = new Vector3(healthBarWidth, healthBarHeight, 1);
        healthBarBG.color = Color.gray;

        healthBarFill.transform.localPosition = new Vector3(-healthBarWidth / 2 - 0.6f, 0, -0.1f);
        healthBarFill.transform.localScale = new Vector3(healthBarWidth, healthBarHeight, 1);
        healthBarFill.color = fullHealthColor;

        SetSpritePivotToLeft(healthBarFill.sprite);
    }

    private void SetSpritePivotToLeft(Sprite originalSprite)
    {
        if (originalSprite == null) return;
        
        Sprite newSprite = Sprite.Create(
            originalSprite.texture,
            originalSprite.rect,
            new Vector2(0, 0.5f),
            originalSprite.pixelsPerUnit
        );
        healthBarFill.sprite = newSprite;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || _currentHealth <= 0) return;

        int actualDamage = damage;
        
        if (useShield)
        {
            if (remainingShieldHits > 0)
            {
                remainingShieldHits--;
                
                actualDamage = Mathf.RoundToInt(damage * (1f - damageReductionPercent));
                
                if (shieldEffectPrefab != null)
                {
                    ShowShieldEffectWithAnimation();
                }
                
                if (remainingShieldHits <= 0)
                {
                    StopAllCoroutines();
                    HideShieldEffect();
                }
            }
        }

        _currentHealth = Mathf.Max(0, _currentHealth - actualDamage);
        UpdateHealthBarVisual();

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        
        if (hitMarkPrefab != null)
        {
            GameObject mark = Instantiate(hitMarkPrefab, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
            
            SplashHitEffect splashEffect = mark.GetComponent<SplashHitEffect>();
            if (splashEffect != null)
            {
                if (splashEffect.particleSprite == null)
                {
                    SpriteRenderer spriteRenderer = hitMarkPrefab.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        splashEffect.particleSprite = spriteRenderer.sprite;
                    }
                }
            }
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void ShowShieldEffectWithAnimation()
    {
        StopAllCoroutines();
        
        if (currentShieldEffect != null)
        {
            Destroy(currentShieldEffect);
        }
        
        if (shieldEffectPrefab != null)
        {
            currentShieldEffect = Instantiate(shieldEffectPrefab, transform);
            currentShieldEffect.transform.localPosition = Vector3.zero;
            
            SpriteRenderer shieldSprite = currentShieldEffect.GetComponent<SpriteRenderer>();
            if (shieldSprite != null)
            {
                shieldSprite.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, 0f);
            }
            
            StartCoroutine(ShieldAppearAndDisappearAnimation());
        }
    }
    
    private System.Collections.IEnumerator ShieldAppearAndDisappearAnimation()
    {
        if (currentShieldEffect == null)
        {
            yield break;
        }
        
        SpriteRenderer shieldSprite = currentShieldEffect.GetComponent<SpriteRenderer>();
        if (shieldSprite == null)
        {
            yield break;
        }
        
        Transform shieldTransform = currentShieldEffect.transform;
        Vector3 originalScale = shieldTransform.localScale;
        Vector3 targetScale = originalScale * shieldScaleMultiplier;
        Color targetColor = shieldColor;
        
        float appearDuration = shieldEffectDuration / 2f;
        float elapsedTime = 0f;
        
        while (elapsedTime < appearDuration)
        {
            if (currentShieldEffect == null || shieldSprite == null)
            {
                yield break;
            }
            
            float t = elapsedTime / appearDuration;
            shieldTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            shieldSprite.color = Color.Lerp(new Color(targetColor.r, targetColor.g, targetColor.b, 0f), targetColor, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if (currentShieldEffect != null && shieldSprite != null)
        {
            shieldTransform.localScale = targetScale;
            shieldSprite.color = targetColor;
        }
        
        elapsedTime = 0f;
        while (elapsedTime < shieldShowDuration)
        {
            if (currentShieldEffect == null || shieldSprite == null)
            {
                yield break;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        float disappearDuration = shieldEffectDuration / 2f;
        elapsedTime = 0f;
        
        while (elapsedTime < disappearDuration)
        {
            if (currentShieldEffect == null || shieldSprite == null)
            {
                yield break;
            }
            
            float t = elapsedTime / disappearDuration;
            shieldTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            shieldSprite.color = Color.Lerp(targetColor, new Color(targetColor.r, targetColor.g, targetColor.b, 0f), t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        HideShieldEffect();
    }
    
    private void HideShieldEffect()
    {
        if (currentShieldEffect != null)
        {
            Destroy(currentShieldEffect);
            currentShieldEffect = null;
        }
    }

    private void UpdateHealthBarVisual()
    {
        if (healthBarFill == null) return;

        float healthRatio = (float)_currentHealth / maxHealth;
        _fillTransform.localScale = new Vector3(healthRatio * healthBarWidth, healthBarHeight, 1);
        healthBarFill.color = healthRatio <= 0.3f ? lowHealthColor : fullHealthColor;
    }

    private void Die()
    {
        isDead = true;

        if (LevelManager.instance != null)
        {
            LevelManager.instance.AddKillCount();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(reward);
        }
    
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        foreach (MonoBehaviour comp in GetComponentsInChildren<MonoBehaviour>())
        {
            if (comp != this) comp.enabled = false;
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (dieEffectPrefab != null)
        {
            GameObject effect = Instantiate(dieEffectPrefab, transform.position, Quaternion.identity);
            
            DeathExplosionEffect explosionEffect = effect.GetComponent<DeathExplosionEffect>();
            if (explosionEffect != null)
            {
                if (explosionEffect.particleSprite == null)
                {
                    SpriteRenderer spriteRenderer = dieEffectPrefab.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        explosionEffect.particleSprite = spriteRenderer.sprite;
                    }
                }
            }
        }

        // 分裂僵尸
        if (canSplit && splitZombiePrefab != null && splitCount > 0)
        {
            SpawnSplitZombies();
        }

        Destroy(gameObject, 0.6f);
    }

    private void SpawnSplitZombies()
    {
        GameObject unitManager = GameObject.Find("UnitManager");
        Transform parentTransform = unitManager != null ? unitManager.transform : null;

        ZombieMovement originalMovement = GetComponent<ZombieMovement>();

        for (int i = 0; i < splitCount; i++)
        {
            // 计算小僵尸的位置（左右分开）
            float angle = (i * 360f / splitCount) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * splitOffset,
                Mathf.Sin(angle) * splitOffset,
                0
            );
            
            Vector3 spawnPos = transform.position + offset;

            // 生成小僵尸
            GameObject zombie = Instantiate(splitZombiePrefab, spawnPos, Quaternion.identity, parentTransform);

            // 复制 spawnPoint 和 endPoint
            ZombieMovement zombieMove = zombie.GetComponent<ZombieMovement>();
            if (zombieMove != null && originalMovement != null)
            {
                zombieMove.spawnPoint = originalMovement.spawnPoint;
                zombieMove.endPoint = originalMovement.endPoint;
                zombieMove.CalculatePath();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (healthBarRoot != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(healthBarRoot.position, new Vector3(healthBarWidth, healthBarHeight, 0.1f));
        }
    }
}
