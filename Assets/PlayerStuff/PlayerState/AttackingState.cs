using UnityEngine;

public class AttackingState : PlayerStateClass
{
    AbilityCard currentSkill;
    float skillDur;
    float enterTime;
    

    public override void OnStateEnter(PlayerManagerScript player)
    {
        player.MovementScript.canMove = false;
        currentSkill = player.CardManager.currentUsingCard;
        skillDur = currentSkill.Anticipation + currentSkill.Recovery;
        enterTime = Time.time;
    }

    public override void OnStateExit(PlayerManagerScript player)
    {
        if(currentSkill != null)
        {
            currentSkill.StopCoroutine(currentSkill.SkillCoroutine);
        }
        player.CardManager.RotateHand();
        player.MovementScript.canMove = true;
        player.CardManager.currentUsingCard = null;
    }

    public override void OnStatePhysicUpdate(PlayerManagerScript player)
    {

    }

    public override void OnStateUpdate(PlayerManagerScript player)
    {
        if(Time.time - enterTime > skillDur)
        {
            player.ChangeState(player.DefaultState);
        }
    }

    public override void OnUseLeftCard(PlayerManagerScript player)
    {
        
    }
    public override void OnUseRightCard(PlayerManagerScript player)
    {
        
    }
    public override void OnTakeDamage(PlayerManagerScript player, float damage)
    {
        player.CurrentHealth -= damage;
        player.HealthUpdate();
    }
}
