using UnityEngine;

public class MeleeEnemyPatrolState : MeleeEnemyStateClass
{
    float patrolTimer;
    public override void OnStateEnter(MeleeEnemyStateManager meleeEnemy)
    {
        patrolTimer = Random.Range(1f, 5f);
    }

    public override void OnStatePhysicUpdate(MeleeEnemyStateManager meleeEnemy)
    {
        
    }

    public override void OnStateUpdate(MeleeEnemyStateManager meleeEnemy)
    {
        #region Walk
        if(!meleeEnemy.CanWalkFwd())
        {
            meleeEnemy.Flip();
            int rand = Random.Range(0, 1);
            if(rand == 1)
            {
                meleeEnemy.ChangeState(meleeEnemy.IdleState);
                return;
            }
        }
        meleeEnemy.transform.Translate(Vector3.right * meleeEnemy.WalkSpeed * meleeEnemy.FacingDir() * Time.deltaTime);
        #endregion

        #region StateCheck
        patrolTimer -= Time.deltaTime;
        if(patrolTimer < 0)
        {
            meleeEnemy.ChangeState(meleeEnemy.IdleState);
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
