using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManagerScript : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Run,
        HuggingWalls,
        AirAssend,
        AirDescend,
        Dash,
        UsingCard,
        DrawingCard
    }

    public PlayerState State { get; private set; }
    public PlayerMovementScript MovementScript;
    public PlayerAnimationScript AnimationScript;
    public HandManager HandManager;

    public InputActionAsset actionAsset;

    public float drawingCardDur = 1.5f;

    [Header("Health")]
    public float MaxHealth = 100f;
    public float CurrentHealth;
    public Slider HpSlider;

    private PlayerState previousState;
    private bool stateLocked = false;

    private Coroutine drawCardCou;
    private bool isDrawing;

    [HideInInspector] public int facingDir;

    [Header("Ability")]


    public static PlayerManagerScript Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        actionAsset.FindAction("UseCard").started += useCard;
        actionAsset.FindAction("DrawCard").started += drawCard;
    }

    private void Start()
    {
        HandManager.Drawcard();
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        StateCheck();

        if (MovementScript.isFacingRight) facingDir = 1;
        else facingDir = -1;
    }

    private void StateCheck()
    {
        PlayerState newState = State;

        #region state no override
        if (!stateLocked)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            bool isMovingHorizontally = Mathf.Abs(horizontalInput) > 0.01f;

            bool isGrounded = MovementScript.GroundCheck;
            bool isTouchingWall = MovementScript.WallCheck;
            float verticalVelocity = MovementScript.rb.linearVelocityY;
            bool isFalling = verticalVelocity < -0.01f;
            bool isRising = verticalVelocity > 0.01f;
            bool isInAir = !isGrounded;
            bool isDashing = MovementScript.isDashing;

            // Prioritized state checks

            if (isDashing)
            {
                newState = PlayerState.Dash;
            }
            else if (isGrounded)
            {
                newState = isMovingHorizontally ? PlayerState.Run : PlayerState.Idle;
            }
            else if (isInAir && isTouchingWall && isMovingHorizontally && isFalling)
            {
                newState = PlayerState.HuggingWalls;
            }
            else if (isInAir && isRising)
            {
                newState = PlayerState.AirAssend;
            }
            else if (isInAir && isFalling)
            {
                newState = PlayerState.AirDescend;
            }
        }
        #endregion

        if (newState != State)
        {
            previousState = State;
            State = newState;
            stateChanged();
        }
    }

    private void stateChanged()
    {
        //Debug.Log($"State changed from {previousState} to {State}");
    }

    #region Health

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        HealthUpdate();
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        HealthUpdate();
    }

    public void HealthUpdate()
    {
        HpSlider.value = CurrentHealth;
        if(CurrentHealth <= 0)
        {
            Died();
        }
    }

    public void Died()
    {
        Debug.Log("playerDeath");
    }

    #endregion

    #region card
    private void useCard(InputAction.CallbackContext obj)
    {
        HandManager.UseCard();
    }

    #region DrawCard logic
    private void drawCard(InputAction.CallbackContext obj)
    {
        if (State != PlayerState.DrawingCard && MovementScript.GroundCheck && !isDrawing)
        {
            MovementScript.canMove = true;
            stateLocked = false;

            drawCardCou = StartCoroutine(drawingCard());
        }
    }

    public void CancelDrawCard()
    {
        if (isDrawing)
        {
            StopCoroutine(drawCardCou);
            MovementScript.canMove = true;
            stateLocked = false;
            isDrawing = false;
        }
    }

    IEnumerator drawingCard()
    {
        stateLocked = true;
        isDrawing = true;

        State = PlayerState.DrawingCard;
        stateChanged();

        if (MovementScript.isDashing)
        {
            yield return new WaitUntil(() => MovementScript.isDashing == false);
        }
        MovementScript.canMove = false;
        yield return new WaitForSeconds(drawingCardDur);

        MovementScript.canMove = true;
        HandManager.Drawcard();

        stateLocked = false;
        isDrawing = false;
    }
    #endregion
    #endregion

}
