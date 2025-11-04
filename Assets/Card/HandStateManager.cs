using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class HandStateManager : MonoBehaviour
{
    public int CurrentSelectedCard = 1;
    [Space(10)]
    public int MaxHandSize = 6;
    public SplineContainer CurveSplineContainer;
    public SplineContainer StraightSplineContainer;
    [SerializeField] private Transform DeckParent;
    [SerializeField] private Transform discardPoint;
    [SerializeField] private Transform handTransform;
    public float3 offset;

    public bool isSelecting = false;
    public List<GameObject> DeckPile = new();
    public List<GameObject> handpile = new();
    public List<GameObject> DiscardedPile = new();
    private float selectedTime = 3f;

    [Header("Rearrange state stuff")]
    public float CameraMoveSpeed;
    public Vector2 maxFreeCamDist;

    public HandState CurrentState;

    public DefaultHandState DefaultHandState = new DefaultHandState();
    public ReserveState ReserveState = new ReserveState();
    public RearrangingState rearrangingState = new RearrangingState();

    [HideInInspector] public float scrollIdleTimer = 0f;
    [HideInInspector] public SplineContainer CurrentSplineContainer;

    private void Awake()
    {
        for (int i = 0; i < DeckParent.childCount; i++)
        {
            DeckPile.Add(DeckParent.GetChild(i).gameObject);
        }
        DOTween.SetTweensCapacity(1250, 50);

        CurrentSplineContainer = CurveSplineContainer;
    }

    private void Start()
    {
        CurrentState = DefaultHandState;
        CurrentState.OnStateEnter(this);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState.OnStateUpdate(this);
    }

    public void ChangeState(HandState state)
    {
        CurrentState.OnStateExit(this);
        CurrentState = state;
        Debug.Log("Change hand state to " + state);
        CurrentState.OnStateEnter(this);
    }

    public void ChooseCard()
    {
        if (isSelecting)
        {
            CurrentState.OnSelectCard(this, handpile[CurrentSelectedCard].GetComponent<AbilityCard>());
        }
    }

    public void ResetSelectionTimer()
    {
        scrollIdleTimer = selectedTime;
    }

    public void Drawcard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (DeckPile.Count > 0)
            {
                GameObject card = DeckPile[0];

                handpile.Add(card);
                DeckPile.RemoveAt(0);
            }
            else break;
        }

        if (isSelecting) isSelecting = false;
        updateHandCardPos();
    }

    public void ClearHand()
    {
        int handsize = handpile.Count;
        for (int i = 0; i < handsize; i++)
        {
            discardCard(handpile[0]);
        }
    }

    public void ResetCard()
    {
        DeckPile.AddRange(handpile);
        DeckPile.AddRange(DiscardedPile);


        foreach (GameObject card in handpile)
        {
            PutBackToDeckAnim(card);
        }

        foreach (GameObject card in DiscardedPile)
        {
            PutBackToDeckAnim(card);
        }

        shuffleDeck();

        handpile.Clear();
        DiscardedPile.Clear();
    }

    public void PutBackToDeckAnim(GameObject card)
    {
        float jumpPow = UnityEngine.Random.Range(300, -300);
        card.transform.DOJump(DeckParent.position, jumpPow, 1, 0.25f);
        card.transform.DORotate(Vector3.zero, 0.25f);
    }

    public void cardScrollSelection()
    {
        if (handpile.Count > 0)
        {
            Vector2 scrollDelta = Input.mouseScrollDelta;
            if (scrollDelta.y != 0)
            {

                if (isSelecting)
                {
                    int totalSelected = CurrentSelectedCard + Mathf.RoundToInt(-scrollDelta.y);
                    if (totalSelected > handpile.Count - 1)
                    {
                        CurrentSelectedCard = totalSelected - handpile.Count/* - 1*/;
                    }
                    else if (totalSelected < 0)
                    {
                        CurrentSelectedCard = handpile.Count + totalSelected;
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
                isSelecting = false;
            }
            updateHandCardPos();
        }
    }

    public void updateHandCardPos()
    {
        if (handpile.Count == 0) return;
        float cardSpacing = 1f / handpile.Count /*maxHandSize*/;
        float firstCardPosition = 0.5f - (handpile.Count - 1) * cardSpacing / 2;
        float selectedPush = 200f;
        Spline spline = CurrentSplineContainer.Spline;

        for (int i = 0; i < handpile.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePos = spline.EvaluatePosition(p) + offset;
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            handpile[i].transform.SetSiblingIndex(i);
            if (isSelecting && i == CurrentSelectedCard)
            {
                splinePos += Vector3.up * selectedPush;
                rotation = quaternion.identity;
                handpile[i].transform.SetAsLastSibling();
            }

            handpile[i].transform.DOMove(splinePos, 0.25f);
            handpile[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);

        }
    }

    public void discardCard(GameObject card)
    {
        handpile.Remove(card);
        DiscardedPile.Add(card);

        float jumpPow = UnityEngine.Random.Range(200, -200);
        card.transform.DOJump(discardPoint.position, jumpPow, 1, 0.25f);

        float rand = UnityEngine.Random.Range(0, 360);
        card.transform.DORotate(new Vector3(0, 0, rand), 0.25f);
    }

    [ContextMenu("Shuffle deck")]
    public void shuffleDeck()
    {
        int deckCount = DeckPile.Count;
        while (deckCount > 1)
        {
            deckCount--;
            int rand = UnityEngine.Random.Range(0, deckCount + 1);
            GameObject temp = DeckPile[rand];
            DeckPile[rand] = DeckPile[deckCount];
            DeckPile[deckCount] = temp;
        }
    }
}
