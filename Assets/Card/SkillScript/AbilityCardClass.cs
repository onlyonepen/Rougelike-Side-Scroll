using System.Collections;
using UnityEngine;

public abstract class AbilityCard : MonoBehaviour
{
    public abstract float Anticipation { get; }
    public abstract float Recovery { get; }
    public Coroutine SkillCoroutine;
    public virtual void UseAbility(PlayerManagerScript playerManager)
    {
        SkillCoroutine = StartCoroutine(Ability(playerManager));
    }
    public abstract IEnumerator Ability(PlayerManagerScript playerManager);
}