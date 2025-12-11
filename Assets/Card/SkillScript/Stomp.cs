using System.Collections;
using UnityEngine;

public class Stomp : AbilityCard
{
    [SerializeField] private float range = 2f;
    [SerializeField] private float damage = 5;
    [SerializeField] private float staggeringTime = 0.2f;
    [SerializeField] private float pushUpForce;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;

    public override void SkillAction()
    {
        Vector3 playerPos = playerManager.MovementScript.transform.position;
        Collider2D[] nearbyEnemy = HitboxVisualizeUtils.Instance.OverlapCircleWithVisualize(playerPos, range, enemyLayer);
        foreach (Collider2D enemy in nearbyEnemy)
        {
            Vector2 direction = enemy.transform.position - playerPos;
            bool isObstructed = Physics2D.Raycast(playerPos, direction, direction.magnitude, groundLayer);
            if (!isObstructed)
            {
                enemy.gameObject.GetComponent<EnemyClass>().TakeDamage(damage, staggeringTime);
                enemy.attachedRigidbody.AddForce(Vector2.up * pushUpForce, ForceMode2D.Impulse);
            }
        }
    }
}
