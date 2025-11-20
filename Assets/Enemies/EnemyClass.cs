using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour, IDamagable
{
    public float MaxHp;
    public float CurrentHp;

    public abstract event Action<string> OnChangeStateDebug;

    public LayerMask groundLayer;
    public LayerMask playerLayer;

    public ReusableEnemyData ReusableData = new ReusableEnemyData();

    //StatusEffectStuff
    [HideInInspector] public bool isOnFire = false;
    [HideInInspector] public bool isFreezing = false;
    [HideInInspector] public bool isMarkedForTomorrow = false;

    [HideInInspector] public Burn burnInstance;
    [HideInInspector] public MarkForTomorrow mFTInstance;
    [HideInInspector] public Freeze freezeInstance;

    public List<StatusClass> StatusEffects = new List<StatusClass>();

    public virtual void TakeDamage(float damage, float staggerTime, bool isCrit = false)
    {
        if(isMarkedForTomorrow) RemoveMarkForTomorrow();

        CurrentHp -= damage;
        LatestDamageDealt.Instance.UpdateDamage(damage, isCrit);

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
        bool isNewEffect = true;
        foreach (StatusClass item in StatusEffects)
        {
            if(item.GetType() == status.GetType())
            {
                item.OnStack();
                isNewEffect = false;
            }
        }

        if (isNewEffect)
        {
            StatusEffects.Add(status);
            status.OnApply(this);
        }
    }

    public void EffectOnUpdate()
    {
        List<StatusClass> toRemove = new List<StatusClass>();
        foreach (StatusClass status in StatusEffects)
        {
            status.OnUpdate();
            if (!status.IsEffectActive)
            {
                status.OnTimedOut();
                toRemove.Add(status);
            }
        }

        foreach(StatusClass status in toRemove)
        {
            StatusEffects.Remove(status);
        }
    }


    public bool GroundCheck()
    {
        Collider2D enemyCol = ReusableData._boxCollider;
        Vector2 BoxcastOrigin = new Vector2(enemyCol.bounds.center.x, enemyCol.bounds.min.y);
        Vector2 BoxcastSize = new Vector2(enemyCol.bounds.size.x * 0.95f, 0.1f);

        return Physics2D.BoxCast(BoxcastOrigin, BoxcastSize, 0, Vector2.zero, Mathf.Infinity, groundLayer);
    }

    public int FacingDir()
    {
        int i = 1;
        if (!ReusableData.IsFacingRight) i = -1;
        return i;
    }

    public virtual void FreezeEnemy(float freezeTime) { }

    public void RemoveMarkForTomorrow()
    {
        mFTInstance.OnTimedOut();
        StatusEffects.Remove(mFTInstance);
        isMarkedForTomorrow = false;
        mFTInstance = null;
    }

    public void RemoveEffect(StatusClass effect)
    {
        foreach(StatusClass status in StatusEffects)
        {
            if (status.GetType() == effect.GetType())
            {
                status.OnTimedOut();
                StatusEffects.Remove(status);
            }
        }
    }
}

public class ReusableEnemyData
{
    [HideInInspector] public bool IsFacingRight;
    [HideInInspector] public BoxCollider2D _boxCollider;
    [HideInInspector] public float attackCooldownTimer = 0;
    [HideInInspector] public float staggerTime = 0;
}