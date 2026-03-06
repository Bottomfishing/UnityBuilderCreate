using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 8f;
    [HideInInspector]
    public int damage;
    public bool isIceBullet = false;
    public float slowDuration = 2f;
    public float slowMultiplier = 0.5f;
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
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position, 
            target.position, 
            bulletSpeed * Time.deltaTime
        );

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
}