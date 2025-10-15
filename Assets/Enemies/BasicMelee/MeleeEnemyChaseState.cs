using UnityEngine;

public class MeleeEnemyChaseState : MeleeEnemyStateClass
{
    private PlayerManagerScript player;
    private float awarenessTimer = 0;
    private float randDistanceToAttack = 0;
    private float randDisplace = 1;
    public override void OnStateEnter(MeleeEnemyStateManager meleeEnemy)
    {
        player = PlayerManagerScript.Instance;

        float actualPlayerDist = Mathf.Abs(meleeEnemy.transform.position.x - player.MovementScript.transform.position.x);
        bool playerOnTheRight = meleeEnemy.transform.position.x < player.MovementScript.transform.position.x;
        if(actualPlayerDist > randDistanceToAttack/* && meleeEnemy.CanWalkFwd()*/)
        {
            randDistanceToAttack = Random.Range(meleeEnemy.DistaneToAttack, meleeEnemy.DistaneToAttack - randDisplace);
        }
    }

    public override void OnStatePhysicUpdate(MeleeEnemyStateManager meleeEnemy)
    {

    }

    public override void OnStateUpdate(MeleeEnemyStateManager meleeEnemy)
    {
        awarenessTimer += Time.deltaTime;
        if (meleeEnemy.PlayerInSight())
        {
            awarenessTimer = 0;
        }

        #region Chase
        float actualPlayerDist = Mathf.Abs(meleeEnemy.transform.position.x - player.MovementScript.transform.position.x);
        bool playerOnTheRight = meleeEnemy.transform.position.x < player.MovementScript.transform.position.x;

        if(actualPlayerDist > randDistanceToAttack && meleeEnemy.CanWalkFwd())
        {
            meleeEnemy.transform.Translate(Vector3.right * meleeEnemy.ChaseSpeed * meleeEnemy.FacingDir() * Time.deltaTime);
        }
        if (playerOnTheRight != meleeEnemy.ReusableData.IsFacingRight)
        {
            meleeEnemy.Flip();
        }
        #endregion

        if (meleeEnemy.CheckAttackCooldown() && meleeEnemy.PlayerInSight() && meleeEnemy.PlayerCheckDistance() < randDistanceToAttack)
        {
            meleeEnemy.ChangeState(meleeEnemy.AttackState);
        }

        if (awarenessTimer > 3f)
        {
            meleeEnemy.ChangeState(meleeEnemy.IdleState);
        }
    }

    public override void OnStateExit(MeleeEnemyStateManager meleeEnemy)
    {

    }
}
