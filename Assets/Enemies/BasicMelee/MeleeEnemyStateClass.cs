public abstract class MeleeEnemyStateClass
{
    public abstract void OnStateEnter(MeleeEnemyStateManager meleeEnemy);
    public abstract void OnStatePhysicUpdate(MeleeEnemyStateManager meleeEnemy);
    public abstract void OnStateUpdate(MeleeEnemyStateManager meleeEnemy);
    public abstract void OnStateExit(MeleeEnemyStateManager meleeEnemy);
}