using System.Collections;
using UnityEngine;

public class MeleeEnemyAttackState : MeleeEnemyStateClass
{
    float anticipation;
    float recovery;

    Coroutine attackCou;

    public override void OnStateEnter(MeleeEnemyStateManager meleeEnemy)
    {
        anticipation = meleeEnemy.Anticipation * meleeEnemy.AttackSpeed;
        recovery = meleeEnemy.Recovery * meleeEnemy.AttackSpeed;

        attackCou = meleeEnemy.StartCoroutine(AttackSequence(meleeEnemy));
    }

    public override void OnStateExit(MeleeEnemyStateManager meleeEnemy)
    {
        if(attackCou != null)
        {
            meleeEnemy.StopCoroutine(attackCou);
        }
    }

    public override void OnStatePhysicUpdate(MeleeEnemyStateManager meleeEnemy)
    {

    }

    public override void OnStateUpdate(MeleeEnemyStateManager meleeEnemy)
    {
        
    }

    public IEnumerator AttackSequence(MeleeEnemyStateManager meleeEnemy)
    {
        yield return new WaitForSeconds(anticipation);

        Debug.Log("Melee attack");
        Bounds boxBound = meleeEnemy.ReusableData._boxCollider.bounds;
        float dTR = meleeEnemy.DistaneToAttack;
        Vector3 attackPos = boxBound.center + (new Vector3(dTR/2 - boxBound.extents.x , 0) * meleeEnemy.FacingDir());
        Vector2 attackSize = new Vector2(dTR + boxBound.extents.x ,1);
        Collider2D[] hit = HitboxVisualizeUtils.Instance.OverlapBoxWithVisualize(attackPos, attackSize, 0, meleeEnemy.playerLayer);

        if (hit.Length != 0)
        {
            PlayerManagerScript.Instance.TakeDamage(meleeEnemy.AttackDamage);
        }

        yield return new WaitForSeconds(recovery);

        meleeEnemy.ReusableData.attackCooldownTimer = meleeEnemy.AttackCooldown * meleeEnemy.AttackSpeed;

        meleeEnemy.ChangeState(meleeEnemy.ChaseState);
    }
}
