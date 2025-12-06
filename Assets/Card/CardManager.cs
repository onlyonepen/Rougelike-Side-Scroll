using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Transform DeckParent;

    public float CardMoveSpeed = 0.25f;

    [Header("Card queue")]
    public Transform CardQueuePos;
    public int MaxShownQueue = 3;
    public float ScaleInQueue;
    public float DistanceForEach;

    public Queue<AbilityCard> CardQueue = new();

    [Header("Hand")]
    public Transform LeftCardPos;
    public Transform RightCardPos;

    public AbilityCard LeftCard;
    public AbilityCard RightCard;

    [Header("Discarded")]
    public Transform DiscardedPos;

    [HideInInspector] public List<AbilityCard> DiscardedList = new();

    PlayerManagerScript playerManager;

    public enum handSide
    {
        left,
        right
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = PlayerManagerScript.Instance;

        for (int i = 0; i < DeckParent.childCount; i++)
        {
            CardQueue.Enqueue(DeckParent.GetChild(i).GetComponent<AbilityCard>());
        }

        DOTween.SetTweensCapacity(1250, 50);
    }

    [Button]
    public void UseCard(handSide side)
    {
        AbilityCard usedCard = RightCard;
        if (side == handSide.left) usedCard = LeftCard;
        usedCard.UseAbility(playerManager);
        RotateHand(side);

        rearrangeCardPos();
    }
    public void ShuffleDeck()
    {
        List<AbilityCard> tempList = CardQueue.ToList();
        int deckCount = tempList.Count;
        while (deckCount > 1)
        {
            deckCount--;
            int rand = UnityEngine.Random.Range(0, deckCount + 1);
            AbilityCard temp = tempList[rand];
            tempList[rand] = tempList[deckCount];
            tempList[deckCount] = temp;
        }
        CardQueue = new Queue<AbilityCard>(tempList);
    }

    private void rearrangeCardPos()
    {
        #region Card queue
        AbilityCard[] queueList = CardQueue.ToArray();
        foreach (AbilityCard item in queueList)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < MaxShownQueue; i++)
        {
            queueList[i].gameObject.SetActive(true);
            Vector3 newpos = CardQueuePos.position + (Vector3.up * DistanceForEach * i);
            queueList[i].transform.DOMove(newpos, CardMoveSpeed);
            Vector3 newScale = Vector3.one * ScaleInQueue;
            queueList[i].transform.DOScale(newScale, CardMoveSpeed);
        }
        #endregion

        #region Hand card
        LeftCard.gameObject.SetActive(true);
        RightCard.gameObject.SetActive(true);

        LeftCard.transform.DOScale(Vector3.one, CardMoveSpeed);
        RightCard.transform.DOScale(Vector3.one, CardMoveSpeed);

        LeftCard.gameObject.transform.DOMove(LeftCardPos.position, CardMoveSpeed);
        RightCard.gameObject.transform.DOMove(RightCardPos.position, CardMoveSpeed);
        #endregion

        #region Discarded card
        foreach (AbilityCard card in DiscardedList)
        {
            if(card.gameObject.activeInHierarchy == true)
            {
                card.gameObject.transform.DOMove(DiscardedPos.position, 0.25f);
            }
        }
        #endregion
    }

    private void RotateHand(handSide side)
    {
        if (side == handSide.left)
        {
            if (LeftCard != null)
            {
                //discardCard
                DiscardedList.Add(LeftCard);
                LeftCard = null;
            }
            if(CardQueue.Count > 0)
            {
                //addCard
                LeftCard = CardQueue.Dequeue().GetComponent<AbilityCard>();
            }
        }

        else //right
        {
            if (RightCard != null)
            {
                //discardCard
                DiscardedList.Add(RightCard);
                RightCard = null;
            }
            if (CardQueue.Count > 0)
            {
                //addCard
                RightCard = CardQueue.Dequeue().GetComponent<AbilityCard>();
            }
        }
    }

    public void SetUpCard()
    {
        ShuffleDeck();

        RotateHand(handSide.left);
        RotateHand(handSide.right);

        rearrangeCardPos();
    }
}
