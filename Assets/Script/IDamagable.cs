using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public abstract void TakeDamage(float damage, float staggerTime);
}
