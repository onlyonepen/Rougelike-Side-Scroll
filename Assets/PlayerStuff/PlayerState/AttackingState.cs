using UnityEngine;

public class AttackingState : PlayerStateClass
{
    
    public override void OnStateEnter(PlayerManagerScript player)
    {
        player.MovementScript.canMove = false;
    }

    public override void OnStateExit(PlayerManagerScript player)
    {
        player.MovementScript.canMove = true;
    }

    public override void OnStatePhysicUpdate(PlayerManagerScript player)
    {

    }

    public override void OnStateUpdate(PlayerManagerScript player)
    {

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
