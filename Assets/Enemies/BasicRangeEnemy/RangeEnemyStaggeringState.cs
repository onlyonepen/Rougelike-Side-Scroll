using UnityEngine;

public class RangeEnemyStaggeringState : RangeEnemyStateClass
{
    float timestamp;
    public override void OnStateEnter(RangeEnemyStateManager rangeEnemy)
    {
        timestamp = Time.time;
    }

    public override void OnStateExit(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStatePhysicUpdate(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStateUpdate(RangeEnemyStateManager rangeEnemy)
    {
        if (Time.time - timestamp >= rangeEnemy.ReusableData.staggerTime)
        {
            rangeEnemy.ChangeState(rangeEnemy.ChaseState);
        }
    }
}
