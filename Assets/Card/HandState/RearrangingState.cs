using UnityEngine;

public class RearrangingState : HandState
{
    public override void OnSelectCard(HandStateManager hand, AbilityCard card)
    {
        
    }
    public override void OnStateEnter(HandStateManager hand)
    {
        PlayerManagerScript.Instance.MovementScript.canMove = false;
        hand.offset.y += 200;
    }
    public override void OnStateUpdate(HandStateManager hand)
    {
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            hand.ChangeState(hand.DefaultHandState);
        }


        if (hand.isSelecting)
        {
            if (Input.GetKeyDown(KeyCode.E)) //moveright
            {
                int currentSelectedIndex = hand.CurrentSelectedCard;
                GameObject selectedCard = hand.handpile[currentSelectedIndex];
                hand.handpile.RemoveAt(currentSelectedIndex);
                hand.handpile.Insert(currentSelectedIndex + 1, selectedCard);
                hand.scrollIdleTimer = 0f;
                hand.CurrentSelectedCard++;
            }
            else if (Input.GetKeyDown(KeyCode.Q)) //moveleft
            {
                int currentSelectedIndex = hand.CurrentSelectedCard;
                GameObject selectedCard = hand.handpile[currentSelectedIndex];
                hand.handpile.RemoveAt(currentSelectedIndex);
                hand.handpile.Insert(currentSelectedIndex - 1, selectedCard);
                hand.scrollIdleTimer = 0f;
                hand.CurrentSelectedCard--;
            }
        }
        hand.cardScrollSelection();

        //freecam
    }

    public override void OnStateExit(HandStateManager hand)
    {
        //reset selection

        PlayerManagerScript.Instance.MovementScript.canMove = true;

        hand.offset.y -= 200;
    }

}
