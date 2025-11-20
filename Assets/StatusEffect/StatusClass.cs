using UnityEngine;

public abstract class StatusClass
{
    public bool IsEffectActive = true;
    public abstract void OnApply(EnemyClass enemyClass);
    public abstract void OnUpdate();
    public abstract void OnTimedOut();
    public abstract void OnStack();
}
