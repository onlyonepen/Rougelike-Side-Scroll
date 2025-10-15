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
        Vector3 attackPos = boxBound.center + (new Vector3(meleeEnemy.DistaneToAttack , 0) * meleeEnemy.FacingDir());
        Vector2 attackSize = new Vector2(boxBound.extents.x, boxBound.size.y);
        bool HitPlayer = Physics2D.OverlapBox(attackPos, attackSize, meleeEnemy.playerLayer);
        if (HitPlayer)
        {
            PlayerManagerScript.Instance.TakeDamage(meleeEnemy.AttackDamage);
        }

        yield return new WaitForSeconds(recovery);

        meleeEnemy.ReusableData.attackCooldownTimer = meleeEnemy.AttackCooldown * meleeEnemy.AttackSpeed;

        meleeEnemy.ChangeState(meleeEnemy.ChaseState);
    }
}
