using UnityEngine;

public class Freeze : StatusClass
{
    public float FreezeTime = 2;

    private EnemyClass enemy;
    public override void OnApply(EnemyClass enemyClass)
    {
        enemy = enemyClass;
        enemy.isFreezing = true;
        enemy.FreezeEnemy(FreezeTime);
    }

    public override void OnUpdate()
    {

    }

    public override void OnTimedOut()
    {
        enemy.isFreezing = false;
    }

    public override void OnStack() 
    {
        
    }
}
