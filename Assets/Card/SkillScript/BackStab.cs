using UnityEngine;
using System.Collections;

public class BackStab : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 1f; } }
    public float Range = 2f;
    public float Damage = 5f;
    public float CritMult = 2f;
    public float StaggeringTime = 0.5f;
    [SerializeField] private float pushUpForce;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 castOrigin;
    private Vector2 castSize = new Vector2(1, 0.5f);
    private float castAngle = 0f;
    private bool isAttacking = false;

    public override void UseAbility(PlayerManagerScript playerManager)
    {
        StartCoroutine(Ability(playerManager));
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        Debug.Log("UseSkill");
        yield return new WaitForSeconds(Anticipation);

        isAttacking = true;
        Collider2D[] castHit;
        Vector3 playerPos = playerManager.MovementScript.transform.position;
        Vector3 attackOffset = Vector3.right * playerManager.facingDir * Range / 2;

        castOrigin = attackOffset + playerPos;

        castHit = Physics2D.OverlapBoxAll(castOrigin, castSize, castAngle, enemyLayer);

        bool hitEnemy = false;

        foreach (Collider2D hit in castHit)
        {
            Vector2 direction = hit.transform.position - playerPos;
            bool isObstructed = Physics2D.Raycast(playerPos, direction, direction.magnitude, groundLayer);
            if (!isObstructed)
            {
                float totalDmg = Damage;
                if (hit.transform.TryGetComponent<EnemyClass>(out EnemyClass enemyClass))
                {
                    if (enemyClass.FacingDir() == playerManager.facingDir) totalDmg *= CritMult;
                }
                hit.gameObject.GetComponent<EnemyClass>().TakeDamage(totalDmg, StaggeringTime);
                hit.attachedRigidbody.AddForce(Vector2.up * pushUpForce, ForceMode2D.Impulse);
            }

            hitEnemy = true;
        }

        if (hitEnemy)
        {

        }

        yield return new WaitForSeconds(Recovery);

        isAttacking = false;
    }
}
