using DG.Tweening;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int CurrentSelectedCard = 1;
    [Space (10)]
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform DeckParent;
    [SerializeField] private Transform discardPoint;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float3 offset;

    public bool isSelecting = false;
    public List<GameObject> Deck = new();
    public List<GameObject> handCard = new();
    public List<GameObject> DiscardedCard = new();
    private float selectedTime = 3f;

    float scrollIdleTimer = 0f;

    private void Awake()
    {
        for(int i = 0; i < DeckParent.childCount; i++)
        {
            Deck.Add(DeckParent.GetChild(i).gameObject);
        }
        DOTween.SetTweensCapacity(1250, 50);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R)) Drawcard();

        if (Input.GetKeyDown(KeyCode.G)) ResetCard();

        cardScrollSelection();

        //UseCard();
    }

    public void Drawcard()
    {
        int cardleft = handCard.Count;
        for (int i =  0; i < cardleft; i++)
        {
            DiscardedCard.Add(handCard[0]);

            discardCard(handCard[0]);
            //handCard[0].SetActive(false);

            handCard.RemoveAt(0);
        }
        
        for (int i = 0; i < maxHandSize; i++)
        {
            if (Deck.Count > 0)
            {
                int rand = UnityEngine.Random.Range(0, Deck.Count - 1);
                GameObject card = Deck[rand];

                //GameObject _obj = Instantiate(card, spawnPoint.position, spawnPoint.rotation, handTransform);

                handCard.Add(card);
                Deck.RemoveAt(rand);
            }
            else break;
        }

        if (isSelecting) isSelecting = false;
        updateHandCardPos();
    }

    public void ResetCard()
    {
        Deck.AddRange(handCard);
        Deck.AddRange(DiscardedCard);

        foreach (GameObject card in handCard)
        {
            putBackToDeck(card);
        }

        foreach (GameObject card in DiscardedCard)
        {
            putBackToDeck(card);
        }

        handCard.Clear();
        DiscardedCard.Clear();
    }

    private void putBackToDeck(GameObject card)
    {
        float jumpPow = UnityEngine.Random.Range(300, -300);
        card.transform.DOJump(DeckParent.position, jumpPow, 1, 0.25f);
        //card.transform.DOMove(DeckParent.position, 0.25f);
        card.transform.DORotate(Vector3.zero, 0.25f);
    }

    public void UseCard()
    {
        if (/*Input.GetMouseButtonDown(0) &&*/ isSelecting && (CurrentSelectedCard >= 0 && CurrentSelectedCard < handCard.Count))
        {
            PlayerManagerScript playerManager = PlayerManagerScript.Instance;
            handCard[CurrentSelectedCard].GetComponent<AbilityCard>().UseAbility(playerManager);
            //discard Card
            GameObject usedCard = handCard[CurrentSelectedCard];
            DiscardedCard.Add(usedCard);
            handCard.RemoveAt(CurrentSelectedCard);
            discardCard(usedCard);
            scrollIdleTimer = selectedTime;
        }
    }

    private void cardScrollSelection()
    {
        if(handCard.Count > 0)
        {
            Vector2 scrollDelta = Input.mouseScrollDelta;
            if (scrollDelta.y != 0)
            {

                if (isSelecting)
                {
                    int totalSelected = CurrentSelectedCard + Mathf.RoundToInt(-scrollDelta.y);
                    if (totalSelected > handCard.Count - 1)
                    {
                        CurrentSelectedCard = totalSelected - handCard.Count/* - 1*/;
                    }
                    else if (totalSelected < 0)
                    {
                        CurrentSelectedCard = handCard.Count + totalSelected;
                    }
                    else
                    {
                        CurrentSelectedCard = totalSelected;
                    }
                }
                scrollIdleTimer = 0f;
                isSelecting = true;
            }

            scrollIdleTimer += Time.deltaTime;
            if (scrollIdleTimer > selectedTime && isSelecting)
            {
                //CurrentSelectedCard = handCard.Count; //deselect
                isSelecting = false;
                //updateHandCardPos();
            }
            updateHandCardPos();
        }
    }

    private void updateHandCardPos()
    {
        if (handCard.Count == 0) return;
        float cardSpacing = 1f / handCard.Count /*maxHandSize*/;
        float firstCardPosition = 0.5f - (handCard.Count - 1) * cardSpacing / 2;
        float selectedPush = 200f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCard.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePos = spline.EvaluatePosition(p) + offset;
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            handCard[i].transform.SetSiblingIndex(i);
            if (isSelecting && i == CurrentSelectedCard)
            {
                splinePos += Vector3.up * selectedPush;
                rotation = quaternion.identity;
                handCard[i].transform.SetAsLastSibling();
            }

            handCard[i].transform.DOMove(splinePos, 0.25f);
            handCard[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);

        }
    }

    private void discardCard(GameObject card)
    {
        //card.transform.DOMove(discardPoint.position, 0.25f);
        float jumpPow = UnityEngine.Random.Range(200, -200);
        card.transform.DOJump(discardPoint.position, jumpPow, 1, 0.25f);

        float rand = UnityEngine.Random.Range(0, 360);
        card.transform.DORotate(new Vector3(0, 0, rand), 0.25f);
    }
}
