using UnityEngine;

public class RangeEnemyIdleState : RangeEnemyStateClass
{
    float idleTimer;

    public override void OnStateEnter(RangeEnemyStateManager rangeEnemy)
    {
        idleTimer = Random.Range(1f, 5f);
    }

    public override void OnStateExit(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStatePhysicUpdate(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStateUpdate(RangeEnemyStateManager rangeEnemy)
    {
        #region StateCheck
        idleTimer -= Time.deltaTime;
        if (idleTimer < 0)
        {
            rangeEnemy.ChangeState(rangeEnemy.PatrolState);
        }

        if (rangeEnemy.PlayerInSight())
        {
            rangeEnemy.ChangeState(rangeEnemy.ChaseState);
        }
        #endregion
    }
}
