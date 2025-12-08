using UnityEngine;

public abstract class PlayerStateClass
{
    public abstract void OnStateEnter(PlayerManagerScript player);
    public abstract void OnStatePhysicUpdate(PlayerManagerScript player);
    public abstract void OnUseLeftCard(PlayerManagerScript player);
    public abstract void OnUseRightCard(PlayerManagerScript player);
    public abstract void OnStateUpdate(PlayerManagerScript player);
    public abstract void OnStateExit(PlayerManagerScript player);
    public abstract void OnTakeDamage(PlayerManagerScript player, float damage);
}
