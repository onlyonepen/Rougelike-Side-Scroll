using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManagerScript : MonoBehaviour
{
    public PlayerMovementScript MovementScript;
    public PlayerAnimationScript AnimationScript;
    public CardManager CardManager;

    public InputActionAsset actionAsset;

    public float drawingCardDur = 1.5f;

    [Header("Health")]
    public float MaxHealth = 100f;
    public float CurrentHealth;
    public Slider HpSlider;

    [HideInInspector] public int facingDir;

    public PlayerStateClass CurrentState;
    public PlayerStateClass DefaultState = new NormalState();
    public PlayerStateClass AttackingState = new AttackingState();

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
        actionAsset.FindAction("UseLeftCard").started += useLeftCard;
        actionAsset.FindAction("UseRightCard").started += useRightCard;
    }

    private void Start()
    {
        CurrentState = DefaultState;
        CurrentState.OnStateEnter(this);

        CardManager.SetUpCard();
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        CurrentState.OnStateUpdate(this);

        if (MovementScript.isFacingRight) facingDir = 1;
        else facingDir = -1;
    }

    private void FixedUpdate()
    {
        CurrentState.OnStatePhysicUpdate(this);
    }

    public void ChangeState(PlayerStateClass state)
    {
        CurrentState.OnStateExit(this);
        CurrentState = state;
        CurrentState.OnStateEnter(this);
    }

    #region Health

    public void TakeDamage(float damage)
    {
        CurrentState.OnTakeDamage(this, damage);
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
    public void useLeftCard(InputAction.CallbackContext obj)
    {
        CurrentState.OnUseLeftCard(this);
    }

    public void useRightCard(InputAction.CallbackContext obj)
    {
        CurrentState.OnUseRightCard(this);
    }
    #endregion
}
