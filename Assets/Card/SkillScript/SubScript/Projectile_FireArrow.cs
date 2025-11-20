using UnityEngine;

public class Projectile_FireArrow : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float staggeringTime;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damageable))
        {
            damageable.TakeDamage(damage, staggeringTime);
            if (collision.gameObject.TryGetComponent<EnemyClass>(out EnemyClass enemyClass))
            {
                Burn _burn = new Burn();
                enemyClass.burnInstance = _burn;
                enemyClass.ApplyStatusEffect(_burn);
            }
        }

        Destroy(gameObject);
    }
}
