using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    [Header("攻击设置")]
    public float attackRange = 3f;
    public float attackInterval = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int bulletDamage = 1;
    
    [Header("范围伤害设置")]
    public bool useAreaDamage = false;
    public float explosionRadius = 2f;
    public GameObject explosionEffectPrefab;
    
    [Header("目标设置")]
    public string targetTag = "Zombie";
    
    private float attackTimer = 0f;
    
    private void Start()
    {
        if (firePoint == null)
        {
            firePoint = transform;
        }
        attackTimer = 0;
    }
    
    private void Update()
    {
        attackTimer += Time.deltaTime;
        
        GameObject target = FindNearestTarget();
        if (target != null && attackTimer >= attackInterval)
        {
            FireBullet(target);
            attackTimer = 0f;
        }
    }
    
    private GameObject FindNearestTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        GameObject nearestTarget = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag(targetTag))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = collider.gameObject;
                }
            }
        }
        
        return nearestTarget;
    }
    
    private void FireBullet(GameObject targetZombie)
    {
        if (bulletPrefab == null || targetZombie == null)
        {
            return;
        }
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetTarget(targetZombie.transform);
            bulletScript.SetDamage(bulletDamage);
            
            if (useAreaDamage)
            {
                bulletScript.isAreaDamage = true;
                bulletScript.explosionRadius = explosionRadius;
                bulletScript.explosionEffectPrefab = explosionEffectPrefab;
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (useAreaDamage)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
