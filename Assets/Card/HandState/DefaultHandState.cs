using UnityEngine;

public class DefaultHandState : HandState
{
    public override void OnSelectCard(HandStateManager hand, AbilityCard card)
    {
        card.UseAbility(PlayerManagerScript.Instance);
        hand.discardCard(card.gameObject);
        hand.ResetSelectionTimer();
    }

    public override void OnStateEnter(HandStateManager hand)
    {

    }
    public override void OnStateUpdate(HandStateManager hand)
    {
        hand.cardScrollSelection();
    }

    public override void OnStateExit(HandStateManager hand)
    {

    }

}
