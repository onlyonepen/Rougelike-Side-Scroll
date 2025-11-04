using DG.Tweening;
using UnityEngine;

public class RearrangingState : HandState
{
    Vector2 cameraOffset = Vector2.zero;
    Vector2 cameraOriginPos;
    public override void OnSelectCard(HandStateManager hand, AbilityCard card)
    {
        
    }
    public override void OnStateEnter(HandStateManager hand)
    {
        hand.CurrentSplineContainer = hand.StraightSplineContainer;

        PlayerManagerScript.Instance.MovementScript.canMove = false;
        hand.offset.y += 200;

        cameraOffset = Vector2.zero;
        cameraOriginPos = CameraManager.Instance.MainCameraPos.transform.position;
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
                int newspot = (hand.CurrentSelectedCard + 1) % hand.handpile.Count;
                GameObject selectedCard = hand.handpile[hand.CurrentSelectedCard];
                hand.handpile.RemoveAt(hand.CurrentSelectedCard);
                hand.handpile.Insert(newspot, selectedCard);
                hand.scrollIdleTimer = 0f;
                hand.CurrentSelectedCard = newspot;

            }
            else if (Input.GetKeyDown(KeyCode.Q)) //moveleft
            {
                int newspot = (hand.CurrentSelectedCard + hand.handpile.Count - 1) % hand.handpile.Count;
                GameObject selectedCard = hand.handpile[hand.CurrentSelectedCard];
                hand.handpile.RemoveAt(hand.CurrentSelectedCard);
                hand.handpile.Insert(newspot, selectedCard);
                hand.scrollIdleTimer = 0f;
                hand.CurrentSelectedCard = newspot;
            }
        }
        hand.cardScrollSelection();

        #region freecam
        float horizontalinput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        float desireY = cameraOffset.y + verticalInput * hand.CameraMoveSpeed * Time.deltaTime;
        float desireX = cameraOffset.x + horizontalinput * hand.CameraMoveSpeed * Time.deltaTime;
        if(Mathf.Abs(desireX) < hand.maxFreeCamDist.x)
        {
            cameraOffset.x = desireX;
        }
        if (Mathf.Abs(desireY) < hand.maxFreeCamDist.y)
        {
            cameraOffset.y = desireY;
        }
        CameraManager.Instance.MainCameraPos.transform.position = cameraOriginPos + cameraOffset;
        #endregion
    }

    public override void OnStateExit(HandStateManager hand)
    {
        CameraManager.Instance.MainCameraPos.transform.DOLocalMove(Vector2.zero, 0.5f);

        PlayerManagerScript.Instance.MovementScript.canMove = true;

        hand.offset.y -= 200;
    }

}
