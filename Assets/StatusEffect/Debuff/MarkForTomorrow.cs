using UnityEngine;

public class MarkForTomorrow : StatusClass
{
    private EnemyClass enemy;
    public override void OnApply(EnemyClass enemyClass)
    {
        enemy = enemyClass;
        enemy.isMarkedForTomorrow = true;
        enemy.mFT = this;
    }

    public override void OnUpdate()
    {

    }

    public override void OnTimedOut()
    {

    }
}
