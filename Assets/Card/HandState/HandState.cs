using UnityEngine;

public abstract class HandState
{
    public abstract void OnSelectCard(HandStateManager hand, AbilityCard card);
    public abstract void OnStateEnter(HandStateManager hand);
    public abstract void OnStateUpdate(HandStateManager hand);
    public abstract void OnStateExit(HandStateManager hand);
}