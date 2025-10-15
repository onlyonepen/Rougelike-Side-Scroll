using System.Collections;
using UnityEngine;

public class Stomp : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 1f; } }

    [SerializeField] private float range = 3f;
    [SerializeField] private float damage = 5;
    [SerializeField] private float staggeringTime = 0.2f;
    [SerializeField] private float pushUpForce;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;

    public override void UseAbility(PlayerManagerScript playerManager)
    {
        StartCoroutine(Ability(playerManager));
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        yield return new WaitForSeconds(Anticipation);

        Debug.Log("UseStomp");

        Vector3 playerPos = playerManager.MovementScript.transform.position;
        Collider2D[] nearbyEnemy = Physics2D.OverlapCircleAll(playerPos, range, enemyLayer);
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

        yield return new WaitForSeconds(Recovery);
    }
}
