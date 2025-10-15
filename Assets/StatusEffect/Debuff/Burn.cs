using UnityEngine;

public class Burn : StatusClass
{
    private float burnDmg = 1f;
    private float burninterval = 1f;

    float lastBurnTime = 0f;
    EnemyClass enemy;

    public override void OnApply(EnemyClass enemyClass)
    {
        Debug.Log("ApplyBurn");
        enemy = enemyClass;
        enemy.isOnFire = true;
    }

    public override void OnUpdate()
    {
        if (Time.time - lastBurnTime > burninterval)
        {
            enemy.TakeDamage(burnDmg, 0);
            lastBurnTime = Time.time;
        }
    }

    public override void OnTimedOut()
    {

    }
}
