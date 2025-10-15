using System;
using UnityEngine;

public class DummyScript : EnemyClass
{
    public override event Action<string> OnChangeStateDebug;

    private void OnEnable()
    {
        CurrentHp = MaxHp;
    }

    private void Update()
    {
        EffectOnUpdate();
    }
}
