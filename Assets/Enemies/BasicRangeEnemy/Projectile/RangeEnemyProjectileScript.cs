using UnityEngine;

public class RangeEnemyProjectileScript : MonoBehaviour
{
    [HideInInspector] public float damage;
    //[HideInInspector] public float staggeringTime;
    [HideInInspector] public LayerMask playerLayer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            PlayerManagerScript.Instance.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
