using System;
using System.Collections;
using UnityEngine;

public abstract class AbilityCard : MonoBehaviour
{
    [SerializeField] protected float Anticipation = 0.5f;
    [SerializeField] protected float Recovery = 0.2f;
    public Coroutine SkillCoroutine;

    private Action EventSkillExcuted;
    private Action EventSkillEnded;

    protected PlayerManagerScript playerManager;

    public virtual void UseAbility(Action Excuted, Action Ended)
    {
        EventSkillExcuted = Excuted;
        EventSkillEnded = Ended;
        playerManager = PlayerManagerScript.Instance;

        SkillCoroutine = StartCoroutine(Ability());
    }
    public IEnumerator Ability()
    {
        Debug.Log("UseSkill");

        yield return new WaitForSeconds(Anticipation);
        SkillAction();
        EventSkillExcuted?.Invoke();
        yield return new WaitForSeconds(Recovery);
        EventSkillEnded?.Invoke();

        EventSkillExcuted = null;
        EventSkillEnded = null;
    }

    public virtual void CancelSkill()
    {
        StopCoroutine(SkillCoroutine);

        EventSkillEnded?.Invoke();
        EventSkillExcuted = null;
        EventSkillEnded = null;
    }

    public abstract void SkillAction();
}