using System.Collections;
using UnityEngine;

public class MeleeEnemyStaggeringState : MeleeEnemyStateClass
{
    float timestamp;
    public override void OnStateEnter(MeleeEnemyStateManager meleeEnemy)
    {
        timestamp = Time.time;
    }

    public override void OnStateExit(MeleeEnemyStateManager meleeEnemy)
    {

    }

    public override void OnStatePhysicUpdate(MeleeEnemyStateManager meleeEnemy)
    {
    }

    public override void OnStateUpdate(MeleeEnemyStateManager meleeEnemy)
    {
        if(Time.time - timestamp >= meleeEnemy.ReusableData.staggerTime)
        {
            meleeEnemy.ChangeState(meleeEnemy.ChaseState);
        }
    }
}
