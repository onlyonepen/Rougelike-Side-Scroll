using UnityEngine;

public class MeleeEnemyFreezeState : MeleeEnemyStateClass
{
    public float FreezeTimer = 0;
    public override void OnStateEnter(MeleeEnemyStateManager meleeEnemy)
    {
        meleeEnemy.isFreezing = true;
    }

    public override void OnStateUpdate(MeleeEnemyStateManager meleeEnemy)
    {
        FreezeTimer -= Time.deltaTime;
        if (FreezeTimer <= 0)
        {
            meleeEnemy.ChangeState(meleeEnemy.ChaseState);
        }
    }

    public override void OnStatePhysicUpdate(MeleeEnemyStateManager meleeEnemy)
    {

    }

    public override void OnStateExit(MeleeEnemyStateManager meleeEnemy)
    {
        meleeEnemy.isFreezing = false;
    }
}
