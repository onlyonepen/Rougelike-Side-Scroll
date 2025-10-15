using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour, IDamagable
{
    public float MaxHp;
    public float CurrentHp;
    [HideInInspector] public bool isOnFire = false;
    [HideInInspector] public bool isFreezing = false;

    public List<StatusClass> StatusEffects = new List<StatusClass>();

    public virtual void TakeDamage(float damage, float staggerTime)
    {
        CurrentHp -= damage;
        Debug.Log("Take " + damage + " damage");

        if (CurrentHp <= 0)
        {
            Died();
        }
    }


    public void Died()
    {
        Destroy(gameObject);
    }


    public void ApplyStatusEffect(StatusClass status)
    {
        StatusEffects.Add(status);
    }

    public void EffectOnUpdate()
    {
        foreach (StatusClass status in StatusEffects)
        {
            status.OnUpdate();
        }
    }

    public abstract event Action<string> OnChangeStateDebug;

    public LayerMask groundLayer;
    public LayerMask playerLayer;

    public ReusableEnemyData ReusableData = new ReusableEnemyData();

    public bool GroundCheck()
    {
        Collider2D enemyCol = ReusableData._boxCollider;
        Vector2 BoxcastOrigin = new Vector2(enemyCol.bounds.center.x, enemyCol.bounds.min.y);
        Vector2 BoxcastSize = new Vector2(enemyCol.bounds.size.x * 0.95f, 0.1f);

        return Physics2D.BoxCast(BoxcastOrigin, BoxcastSize, 0, Vector2.zero, Mathf.Infinity, groundLayer);
    }

    public virtual void FreezeEnemy(float freezeTime) { }
}

//public abstract class EnemyStateManager : EnemyClass
//{
//    public abstract event Action<string> OnChangeStateDebug;

//    public LayerMask groundLayer;
//    public LayerMask playerLayer;

//    public ReusableEnemyData ReusableData = new ReusableEnemyData();

//    public bool GroundCheck()
//    {
//        Collider2D enemyCol = ReusableData._boxCollider;
//        Vector2 BoxcastOrigin = new Vector2(enemyCol.bounds.center.x, enemyCol.bounds.min.y);
//        Vector2 BoxcastSize = new Vector2(enemyCol.bounds.size.x * 0.95f, 0.1f);

//        return Physics2D.BoxCast(BoxcastOrigin, BoxcastSize, 0, Vector2.zero, Mathf.Infinity, groundLayer);
//    }
//}

public class ReusableEnemyData
{
    [HideInInspector] public bool IsFacingRight;
    [HideInInspector] public BoxCollider2D _boxCollider;
    [HideInInspector] public float attackCooldownTimer = 0;
    [HideInInspector] public float staggerTime = 0;
}