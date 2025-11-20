using UnityEngine;

public class MarkForTomorrow : StatusClass
{
    private EnemyClass enemy;
    public override void OnApply(EnemyClass enemyClass)
    {
        enemy = enemyClass;
        enemy.isMarkedForTomorrow = true;
        enemy.mFTInstance = this;
    }

    public override void OnUpdate()
    {

    }

    public override void OnTimedOut()
    {

    }

    public override void OnStack()
    {

    }
}
