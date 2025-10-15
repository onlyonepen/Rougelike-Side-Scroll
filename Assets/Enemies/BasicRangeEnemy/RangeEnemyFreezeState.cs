using UnityEngine;

public class RangeEnemyFreezeState : RangeEnemyStateClass
{
    public float FreezeTimer = 0;
    public override void OnStateEnter(RangeEnemyStateManager rangeEnemy)
    {
        rangeEnemy.isFreezing = true;
    }

    public override void OnStateUpdate(RangeEnemyStateManager rangeEnemy)
    {
        FreezeTimer -= Time.deltaTime;
        if(FreezeTimer <= 0)
        {
            rangeEnemy.ChangeState(rangeEnemy.ChaseState);
        }
    }

    public override void OnStatePhysicUpdate(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStateExit(RangeEnemyStateManager rangeEnemy)
    {
        rangeEnemy.isFreezing = false;
    }

}
