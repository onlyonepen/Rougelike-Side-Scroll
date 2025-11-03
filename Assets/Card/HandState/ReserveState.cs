using UnityEngine;

public class ReserveState : HandState
{
    public override void OnSelectCard(HandStateManager hand, AbilityCard card)
    {
        hand.handpile.RemoveAt(hand.CurrentSelectedCard);
        hand.DeckPile.Insert(0, card.gameObject);
        hand.PutBackToDeckAnim(card.gameObject);
    }
    public override void OnStateEnter(HandStateManager hand)
    {
        UIManager.Instance.DarkenOverlay.SetActive(true);
        PlayerManagerScript.Instance.MovementScript.canMove = false;
    }
    public override void OnStateUpdate(HandStateManager hand)
    {
        hand.cardScrollSelection();
    }

    public override void OnStateExit(HandStateManager hand)
    {
        UIManager.Instance.DarkenOverlay.SetActive(false);
        PlayerManagerScript.Instance.MovementScript.canMove = false;
    }
}
