using UnityEngine;

public class NormalState : PlayerStateClass
{
    public override void OnStateEnter(PlayerManagerScript player)
    {

    }

    public override void OnStateExit(PlayerManagerScript player)
    {

    }

    public override void OnStatePhysicUpdate(PlayerManagerScript player)
    {

    }

    public override void OnStateUpdate(PlayerManagerScript player)
    {

    }
    public override void OnUseRightCard(PlayerManagerScript player)
    {
        player.CardManager.UseCard(CardManager.handSide.right);
        player.ChangeState(player.AttackingState);
    }
    public override void OnUseLeftCard(PlayerManagerScript player)
    {
        player.CardManager.UseCard(CardManager.handSide.left);
        player.ChangeState(player.AttackingState);
    }

    public override void OnTakeDamage(PlayerManagerScript player, float damage)
    {
        player.CurrentHealth -= damage;
        player.HealthUpdate();
    }
}
