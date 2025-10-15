using UnityEngine;

public class Projectile_IceBolt : MonoBehaviour
{
    [HideInInspector] public float damage;
    public float speed = 1;
    [HideInInspector] public float freezeTime = 2;
    [HideInInspector] public int facingDir = 1;

    public float maxAge = 30;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damageable))
        {
            damageable.TakeDamage(damage, 0);
            if (collision.gameObject.TryGetComponent<EnemyClass>(out EnemyClass enemyClass))
            {
                Freeze _freeze = new Freeze();
                _freeze.FreezeTime = freezeTime;
                enemyClass.StatusEffects.Add(_freeze);
                _freeze.OnApply(enemyClass);
            }
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * facingDir * speed * Time.deltaTime);

        maxAge -= Time.deltaTime;
        if(maxAge < 0)
        {
            Destroy(gameObject);
        }
    }
}
