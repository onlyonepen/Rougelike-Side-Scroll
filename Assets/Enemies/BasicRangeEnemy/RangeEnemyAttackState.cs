using System.Collections;
using UnityEngine;

public class RangeEnemyAttackState : RangeEnemyStateClass
{
    float anticipation;
    float recovery;
    GameObject projectile;
    float projDamage;
    LayerMask playerLayer;
    Vector2 throwVec;

    Coroutine attackCou;
    public override void OnStateEnter(RangeEnemyStateManager rangeEnemy)
    {
        anticipation = rangeEnemy.Anticipation * rangeEnemy.AttackSpeed;
        recovery = rangeEnemy.Recovery * rangeEnemy.AttackSpeed;
        projectile = rangeEnemy.Projectile;
        projDamage = rangeEnemy.AttackDamage;
        throwVec = rangeEnemy.ThrowVec;

        attackCou = rangeEnemy.StartCoroutine(AttackSequence(rangeEnemy));
    }

    public override void OnStateExit(RangeEnemyStateManager rangeEnemy)
    {
        if (attackCou != null)
        {
            rangeEnemy.StopCoroutine(attackCou);
        }
    }

    public override void OnStatePhysicUpdate(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStateUpdate(RangeEnemyStateManager rangeEnemy)
    {

    }
    public IEnumerator AttackSequence(RangeEnemyStateManager rangeEnemy)
    {
        yield return new WaitForSeconds(anticipation);

        Debug.Log("Shoot projectile");

        GameObject throwedProj = rangeEnemy.spawnProjectile(projectile);

        if (throwedProj.TryGetComponent<RangeEnemyProjectileScript>(out RangeEnemyProjectileScript enemyProj))
        {
            enemyProj.damage = projDamage;
            playerLayer = rangeEnemy.playerLayer;
            //projectile_Basic.staggeringTime = staggeringTime;
        }
        else Debug.Log("Projectile does not contain specific projectile logic");
        Vector2 _vec = new Vector2(throwVec.x * rangeEnemy.FacingDir(), throwVec.y);
        throwedProj.GetComponent<Rigidbody2D>().AddForce(_vec, ForceMode2D.Impulse);


        yield return new WaitForSeconds(recovery);

        rangeEnemy.ReusableData.attackCooldownTimer = rangeEnemy.AttackCooldown * rangeEnemy.AttackSpeed;

        rangeEnemy.ChangeState(rangeEnemy.ChaseState);
    }
}
