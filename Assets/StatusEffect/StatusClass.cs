using UnityEngine;

public abstract class StatusClass
{
    public abstract void OnApply(EnemyClass enemyClass);
    public abstract void OnUpdate();
    public abstract void OnTimedOut();
}
