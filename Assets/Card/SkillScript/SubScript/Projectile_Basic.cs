using UnityEngine;

public class Projectile_Basic : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float staggeringTime;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damageable))
        {
            damageable.TakeDamage(damage, staggeringTime);
        }

        Destroy(gameObject);
    }
}
