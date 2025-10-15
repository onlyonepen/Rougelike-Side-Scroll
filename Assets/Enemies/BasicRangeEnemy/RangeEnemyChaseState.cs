using UnityEngine;

public class RangeEnemyChaseState : RangeEnemyStateClass
{
    private PlayerManagerScript player;
    private float awarenessTimer = 0;
    private float randDistanceToAttack = 0;
    private float randDisplace = 2;
    public override void OnStateEnter(RangeEnemyStateManager rangeEnemy)
    {
        player = PlayerManagerScript.Instance;
        randDistanceToAttack = Random.Range(rangeEnemy.DistaneToAttack, rangeEnemy.DistaneToAttack - randDisplace);
    }

    public override void OnStatePhysicUpdate(RangeEnemyStateManager rangeEnemy)
    {

    }

    public override void OnStateUpdate(RangeEnemyStateManager rangeEnemy)
    {
        awarenessTimer += Time.deltaTime;
        if (rangeEnemy.PlayerInSight())
        {
            awarenessTimer = 0;
        }

        #region Chase
        float actualPlayerDist = Mathf.Abs(rangeEnemy.transform.position.x - player.MovementScript.transform.position.x);
        bool playerOnTheRight = rangeEnemy.transform.position.x < player.MovementScript.transform.position.x;

        if(actualPlayerDist > randDistanceToAttack && rangeEnemy.CanWalkFwd())
        {
            rangeEnemy.transform.Translate(Vector3.right * rangeEnemy.ChaseSpeed * rangeEnemy.FacingDir() * Time.deltaTime);
        }
        if (playerOnTheRight != rangeEnemy.ReusableData.IsFacingRight)
        {
            rangeEnemy.Flip();
        }
        #endregion

        if (rangeEnemy.CheckAttackCooldown() && rangeEnemy.PlayerInSight() && rangeEnemy.PlayerCheckDistance() < randDistanceToAttack)
        {
            rangeEnemy.ChangeState(rangeEnemy.AttackState);
        }

        if (awarenessTimer > 3f)
        {
            rangeEnemy.ChangeState(rangeEnemy.IdleState);
        }
    }

    public override void OnStateExit(RangeEnemyStateManager rangeEnemy)
    {

    }
}
