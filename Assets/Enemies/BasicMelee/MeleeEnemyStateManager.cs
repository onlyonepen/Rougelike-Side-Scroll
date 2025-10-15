using System;
using UnityEngine;

public class MeleeEnemyStateManager : EnemyClass
{
    public float WalkSpeed = 2;
    public float ChaseSpeed = 4;
    public float DistaneToAttack = 1;
    [Header("Attack stuff")]
    public float AttackSpeed = 1f;
    public float AttackDamage = 10f;
    public float Anticipation = 0.5f;
    public float Recovery = 0.5f;
    public float AttackCooldown = 2f;

    [SerializeField] private string CurrentStateString;
    public MeleeEnemyStateClass CurrentState;

    public MeleeEnemyPatrolState PatrolState = new MeleeEnemyPatrolState();
    public MeleeEnemyIdleState IdleState = new MeleeEnemyIdleState();
    public MeleeEnemyChaseState ChaseState = new MeleeEnemyChaseState();
    public MeleeEnemyAttackState AttackState = new MeleeEnemyAttackState();
    public MeleeEnemyStaggeringState TakeDamageState = new MeleeEnemyStaggeringState();
    public MeleeEnemyFreezeState FreezeState = new MeleeEnemyFreezeState();

    //Temp
    public override event Action<string> OnChangeStateDebug;

    private void Awake()
    {
        ReusableData._boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        CurrentState = IdleState;
        CurrentState.OnStateEnter(this);
    }

    private void OnEnable()
    {
        CurrentHp = MaxHp;
    }

    private void Update()
    {
        CurrentState.OnStateUpdate(this);

        if(ReusableData.attackCooldownTimer > 0) ReusableData.attackCooldownTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CurrentState.OnStatePhysicUpdate(this);
    }

    public void ChangeState(MeleeEnemyStateClass state)
    {
        CurrentState.OnStateExit(this);
        string oldstate = CurrentState.ToString();
        CurrentState = state;
        //Debug.Log("Change state from " +  oldstate + " to " + CurrentState);
        CurrentState.OnStateEnter(this);
        CurrentStateString = CurrentState.ToString();
        OnChangeStateDebug.Invoke(CurrentStateString);
    }

    public override void TakeDamage(float damage, float staggerTime)
    {
        CurrentHp -= damage;
        if(staggerTime > 0)
        {
            ReusableData.staggerTime = staggerTime;
            ChangeState(TakeDamageState);
        }
    }

    public override void FreezeEnemy(float freezeTime)
    {
        FreezeState.FreezeTimer = freezeTime;
        ChangeState(FreezeState);
    }

    public bool CheckAttackCooldown()
    {
        return ReusableData.attackCooldownTimer <= 0;
    }
    private RaycastHit2D raycastCheckPlayer()
    {
        float lookDistance = 15f;
        LayerMask playerAndGround = playerLayer | groundLayer;

        RaycastHit2D playerInSight = Physics2D.Raycast(transform.position, Vector2.right * FacingDir(), lookDistance, playerAndGround);

        return playerInSight;
    }
    public bool PlayerInSight()
    {
        if (!raycastCheckPlayer()) return false;
        return raycastCheckPlayer().collider.gameObject.layer == 7;
    }
    public float PlayerCheckDistance()
    {
        if (raycastCheckPlayer().collider.gameObject.layer != 7) return float.MaxValue;

        return raycastCheckPlayer().distance;
    }
    public bool CanWalkFwd()
    {
        return WalkableRay() && !WallCheckRay();
    }
    public bool WalkableRay()
    {
        Vector3 castOffset = new Vector2(ReusableData._boxCollider.bounds.extents.x * 1.2f * FacingDir(), -ReusableData._boxCollider.bounds.extents.y);
        RaycastHit2D temp = Physics2D.Raycast(transform.position + castOffset, Vector2.down, 0.1f, groundLayer);
        return temp;
    }
    public bool WallCheckRay()
    {
        Vector3 castOffset = new Vector2(ReusableData._boxCollider.bounds.extents.x * FacingDir(), 0);
        RaycastHit2D temp = Physics2D.Raycast(transform.position + castOffset, Vector2.right, ReusableData._boxCollider.bounds.extents.x * 0.2f, groundLayer);
        return temp;
    }
    public int FacingDir()
    {
        int i = 1;
        if (!ReusableData.IsFacingRight) i = -1;
        return i;
    }
    public void Flip()
    {
        Vector3 _localScale = transform.localScale;
        ReusableData.IsFacingRight = !ReusableData.IsFacingRight;
        _localScale.x *= -1f;
        transform.localScale = _localScale;
    }
}