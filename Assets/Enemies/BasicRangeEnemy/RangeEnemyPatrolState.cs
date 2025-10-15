using UnityEngine;

public class RangeEnemyPatrolState : RangeEnemyStateClass
{
    float patrolTimer;
    public override void OnStateEnter(RangeEnemyStateManager rangeEnemy)
    {
        patrolTimer = Random.Range(1f, 5f);
    }

    public override void OnStatePhysicUpdate(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStateUpdate(RangeEnemyStateManager rangeEnemy)
    {
        #region Walk
        if (!rangeEnemy.CanWalkFwd())
        {
            rangeEnemy.Flip();
            int rand = Random.Range(0, 1);
            if (rand == 1)
            {
                rangeEnemy.ChangeState(rangeEnemy.IdleState);
                return;
            }
        }
        rangeEnemy.transform.Translate(Vector3.right * rangeEnemy.WalkSpeed * rangeEnemy.FacingDir() * Time.deltaTime);
        #endregion

        #region StateCheck
        patrolTimer -= Time.deltaTime;
        if (patrolTimer < 0)
        {
            rangeEnemy.ChangeState(rangeEnemy.IdleState);
        }

        if (rangeEnemy.PlayerInSight())
        {
            rangeEnemy.ChangeState(rangeEnemy.ChaseState);
        }
        #endregion
    }

    public override void OnStateExit(RangeEnemyStateManager meleeEnemy)
    {

    }
}