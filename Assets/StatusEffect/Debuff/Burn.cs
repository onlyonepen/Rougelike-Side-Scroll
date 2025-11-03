using UnityEngine;

public class Burn : StatusClass
{
    public float Duration = 5f;

    private float burnDmg = 5f;
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

        Duration -= Time.deltaTime;
        if(Duration <= 0)
        {
            enemy.RemoveEffect(this);
        }
    }

    public override void OnTimedOut()
    {
        Debug.Log("Burn timed out");
    }
}
