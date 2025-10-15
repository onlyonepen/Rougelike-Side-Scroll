using UnityEngine;

public class Freeze : StatusClass
{
    public float FreezeTime = 2;
    public override void OnApply(EnemyClass enemyClass)
    {
        enemyClass.FreezeEnemy(FreezeTime);
    }

    public override void OnUpdate()
    {

    }

    public override void OnTimedOut()
    {

    }
}
