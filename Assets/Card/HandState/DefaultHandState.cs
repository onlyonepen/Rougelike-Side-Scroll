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
        hand.CurrentSplineContainer = hand.CurveSplineContainer;
    }
    public override void OnStateUpdate(HandStateManager hand)
    {
        hand.cardScrollSelection();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            hand.ChangeState(hand.rearrangingState);
        }
    }

    public override void OnStateExit(HandStateManager hand)
    {

    }

}
