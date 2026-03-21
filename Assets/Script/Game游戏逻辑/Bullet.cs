using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 8f;
    [HideInInspector]
    public int damage;
    public bool isIceBullet = false;
    public float slowDuration = 2f;
    public float slowMultiplier = 0.5f;
    public bool isAreaDamage = false;
    public float explosionRadius = 2f;
    public GameObject explosionEffectPrefab;
    private Transform target;
    private bool hasDealtDamage = false;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetDamage(int damageValue)
    {
        damage = damageValue;
    }

    private void Update()
    {
        if (target == null)
        {
            if (isAreaDamage)
            {
                Explode();
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position, 
            target.position, 
            bulletSpeed * Time.deltaTime
        );

        if (isAreaDamage && Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            Explode();
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAreaDamage)
        {
            return;
        }
        
        if (other.CompareTag("Zombie") && !hasDealtDamage)
        {
            hasDealtDamage = true;
            ZombieHealth zombieHealth = other.GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(damage);
            }
            
            if (isIceBullet)
            {
                ZombieMovement zombieMovement = other.GetComponent<ZombieMovement>();
                if (zombieMovement != null)
                {
                    zombieMovement.ApplySlow(slowDuration, slowMultiplier);
                }
            }
            
            Destroy(gameObject);
        }
    }
    
    private void Explode()
    {
        if (hasDealtDamage)
        {
            return;
        }
        
        hasDealtDamage = true;
        
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in hitColliders)
        {
            if (col.CompareTag("Zombie"))
            {
                ZombieHealth zombieHealth = col.GetComponent<ZombieHealth>();
                if (zombieHealth != null)
                {
                    zombieHealth.TakeDamage(damage);
                }
            }
        }
        
        Destroy(gameObject);
    }
}