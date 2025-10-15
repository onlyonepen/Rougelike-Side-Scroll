using UnityEngine;

public class MeleeEnemyIdleState : MeleeEnemyStateClass
{
    float idleTimer;

    public override void OnStateEnter(MeleeEnemyStateManager meleeEnemy)
    {
        idleTimer = Random.Range(1f, 5f);
    }

    public override void OnStatePhysicUpdate(MeleeEnemyStateManager meleeEnemy)
    {

    }

    public override void OnStateUpdate(MeleeEnemyStateManager meleeEnemy)
    {
        #region StateCheck
        idleTimer -= Time.deltaTime;
        if (idleTimer < 0)
        {
            meleeEnemy.ChangeState(meleeEnemy.PatrolState);
        }

        if (meleeEnemy.PlayerInSight())
        {
            meleeEnemy.ChangeState(meleeEnemy.ChaseState);
        }
        #endregion
    }

    public override void OnStateExit(MeleeEnemyStateManager meleeEnemy)
    {
        
    }
}
