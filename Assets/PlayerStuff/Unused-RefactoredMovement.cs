using System.Collections;
using UnityEngine;

public class RefactoredMovement : MonoBehaviour
{
    // --- Public Editor Variables ---
    public float PlayerSpeed = 10f;
    public float JumpForce = 7f;
    public bool CanDoubleJump = false;

    // --- Private Serialized Fields ---
    [Header("Dependencies")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Collider2D playerCollider;

    [Header("Dashing")]
    [SerializeField] private bool hasDash = true;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDur = 0.2f;
    [SerializeField] private float dashCooldown = 0.2f;
    [HideInInspector] public bool isDashing = false;

    [Header("Wall Slide")]
    [SerializeField] private bool hasWallSlide = true;
    [SerializeField] private float wallSlideFac = 0.6f;
    [SerializeField] private bool hasWallJump = true;
    [SerializeField] private float wallJumpPush = 3f;
    [SerializeField] private float wallJumpDur = 0.5f;

    [Header("Coyote, Jump Buffering")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferingTime = 0.2f;

    [Header("Variable Jump Height")]
    [Tooltip("If true, will use velocity cut method. If false, will increase gravity on release jump.")]
    [SerializeField] private bool jumpHeightCut = true;
    [SerializeField] private float gravMultJump = 3f;

    [Header("Faster Falling")]
    [SerializeField] private bool hasFasterFall = true;
    [SerializeField] private float fasterFallMult = 1.5f;

    [Header("Apex Modifier")]
    [SerializeField] private bool hasApexModifier = true;
    [SerializeField] private float velocityDisplaceForApexBonus = 1f;
    [SerializeField] private float apexSpeedMult = 1.4f;
    [SerializeField] private float apexHoverGravMult = 0.5f;

    [Header("Max Fall Speed")]
    [SerializeField] private bool hasMaxFallSpeed = true;
    [SerializeField] private float maxFallSpeed = 15f;

    [Header("Edge Nudge")]
    [SerializeField] private bool hasWallEdgeNudge = true;
    [SerializeField][Range(0f, 1f)] private float maxEmptyHeadWidthToNudge = 2f / 3f;


    // --- Private Fields for State/Calculations ---
    private float coyoteJumpTimer;
    private float bufferJumpTimer;

    [HideInInspector] public bool WallCheck;
    [HideInInspector] public bool GroundCheck;
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public Coroutine DashCoroutine;
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public Rigidbody2D Rb;

    private float currentMoveSpeed;
    private float originalGravScale;
    private float apexSpeed;
    private float apexGravScale;

    private Coroutine horizForceCou;
    private bool isHorizCouRunning = false;

    // A private property for the Rigidbody2D is cleaner than a public field

    void Awake()
    {
        // Use Awake for GetComponent access
        Rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentMoveSpeed = PlayerSpeed;
        apexSpeed = PlayerSpeed * apexSpeedMult;
        originalGravScale = Rb.gravityScale;
        apexGravScale = Rb.gravityScale * apexHoverGravMult;
    }

    void Update()
    {
        // Handle input and basic movement checks
        if (canMove)
        {
            horizontalMovement();
            jumpLogic();
            wallSlide();
            dash();
            Flip();
        }

        // Apply physics modifications *after* the primary movements
        varibleJumpHeight();
        apexModifier();
        fasterFall();
        clampFallSpeed();
        edgeNudge();
    }

    #region Movement Core

    private void horizontalMovement()
    {
        // Using GetAxis for smoothing, but using GetAxisRaw for the wall check direction
        float horizontalInput = Input.GetAxis("Horizontal");
        float horizontalInputRaw = Input.GetAxisRaw("Horizontal");

        // 1. Wall Check
        // The box cast is currently placed *outside* the player.
        // If you intend to use a Raycast/BoxCast to see if there is a wall *to the side*
        // you should use a distance greater than 0.
        // However, since you are moving the transform directly, this check only prevents movement
        // into a wall when the player is pressing against it.

        if (horizontalInputRaw != 0)
        {
            Vector2 BoxcastOrigin = playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x * horizontalInputRaw, 0);
            Vector2 BoxcastSize = new Vector2(0.05f, playerCollider.bounds.size.y);
            WallCheck = Physics2D.BoxCast(BoxcastOrigin, BoxcastSize, 0, Vector2.zero, Mathf.Infinity, groundLayer);
        }
        else
        {
            // Reset WallCheck if no horizontal input to prevent issues
            WallCheck = false;
        }

        // 2. Movement
        if (horizontalInput != 0 && !WallCheck)
        {
            // WARNING: Moving the Transform directly (transform.position += ...) bypasses Unity's physics system.
            // For a Rigidbody2D, it is generally better to use Rb.velocity or Rb.MovePosition for movement.
            // Using transform.position can cause jitter or collision issues with other Rigidbody objects.
            // I'm keeping your original implementation for now, but be aware of this for physics reliability.
            transform.position += new Vector3(horizontalInput * currentMoveSpeed * Time.deltaTime, 0);

            // A more physics-friendly way (assuming a fixed update for velocity):
            // Rb.velocity = new Vector2(horizontalInput * currentMoveSpeed, Rb.velocity.y);
        }
    }

    private void Flip()
    {
        if (!isDashing)
        {
            // Only use GetAxisRaw for immediate flip response
            float horizontalRaw = Input.GetAxisRaw("Horizontal");

            if ((isFacingRight && horizontalRaw < 0f) || (!isFacingRight && horizontalRaw > 0f))
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }

    #endregion

    #region Jump Logic
    private void jumpLogic()
    {
        // 1. Ground Check
        Vector2 boxcastOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 boxcastSize = new Vector2(playerCollider.bounds.size.x * 0.95f, 0.1f);
        // Using a small, downward-extending BoxCast from the bottom edge
        GroundCheck = Physics2D.BoxCast(boxcastOrigin, boxcastSize, 0, Vector2.down, 0.05f, groundLayer);

        // Reset gravity scale when grounded
        if (GroundCheck)
        {
            Rb.gravityScale = originalGravScale;
        }

        // 2. Coyote and Buffering Timers
        if (GroundCheck)
        {
            coyoteJumpTimer = coyoteTime;
        }
        else
        {
            coyoteJumpTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            bufferJumpTimer = jumpBufferingTime;
        }
        else
        {
            bufferJumpTimer -= Time.deltaTime;
        }

        // 3. Perform Jump
        if (coyoteJumpTimer > 0 && bufferJumpTimer > 0)
        {
            performJump();
            coyoteJumpTimer = 0;
            bufferJumpTimer = 0;
            // Additional logic for CanDoubleJump reset would go here
        }
    }

    private void performJump()
    {
        // Zero out Y velocity before applying jump impulse for consistent jump height
        Rb.linearVelocityY = 0;
        Rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    private void varibleJumpHeight()
    {
        if (Input.GetButtonUp("Jump"))
        {
            if (jumpHeightCut)
            {
                // Velocity Cut method: Only cut velocity if moving upwards
                if (Rb.linearVelocityY > 0)
                {
                    Rb.linearVelocityY = 0;
                }
            }
            else
            {
                // Increased Gravity method: Increase gravity scale to bring the player down faster
                Rb.gravityScale = originalGravScale * gravMultJump;
            }
        }
    }
    #endregion

    #region Physics Modifiers

    private void apexModifier()
    {
        if (hasApexModifier)
        {
            // Check if holding jump and near the apex of the jump arch (low vertical velocity)
            if (Input.GetButton("Jump") && Mathf.Abs(Rb.linearVelocityY) < velocityDisplaceForApexBonus)
            {
                currentMoveSpeed = apexSpeed;
                Rb.gravityScale = apexGravScale;
            }
            else
            {
                // Reset to default speed. Gravity reset is handled in GroundCheck, WallSlide, and FasterFall.
                currentMoveSpeed = PlayerSpeed;
            }
        }
    }

    private void fasterFall()
    {
        if (hasFasterFall)
        {
            // Apply faster fall gravity when descending past a certain velocity
            if (Rb.linearVelocityY < -velocityDisplaceForApexBonus && Rb.gravityScale <= originalGravScale)
            {
                // Only increase gravity if it hasn't been increased by the jump-release logic
                Rb.gravityScale = originalGravScale * fasterFallMult;
            }
        }
    }

    private void clampFallSpeed()
    {
        if (hasMaxFallSpeed)
        {
            // Clamp the vertical velocity
            if (Rb.linearVelocityY < -maxFallSpeed)
            {
                Rb.linearVelocity = new Vector2(Rb.linearVelocityX, -maxFallSpeed);
            }
        }
    }

    #endregion

    #region Edge Nudge

    private void edgeNudge()
    {
        if (hasWallEdgeNudge)
        {
            if (Rb.linearVelocityY > 0) //assending
            {
                bool teleported = false;

                float originOffsetDist = 0.1f;

                int facingDir;
                if (isFacingRight) facingDir = 1;
                else facingDir = -1;

                Vector2 raycastOrigin = playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x * facingDir, playerCollider.bounds.extents.y + originOffsetDist);

                Vector2 raycastDir = Vector2.left * facingDir;

                float raycastDistance = playerCollider.bounds.size.x;

                RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, raycastDir, raycastDistance, groundLayer);
                if (hit2D.distance > playerCollider.bounds.size.x * maxEmptyHeadWidthToNudge)
                {
                    if (!Physics2D.Raycast(hit2D.point + (raycastDir * -0.005f), raycastDir * -1, playerCollider.bounds.size.x, groundLayer))
                    {
                        transform.position = new Vector3(hit2D.point.x + ((playerCollider.bounds.extents.x + 0.01f) * facingDir), transform.position.y);
                        teleported = true;
                    }
                }

                if (!teleported)
                {
                    facingDir *= -1;

                    raycastOrigin = playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x * facingDir, playerCollider.bounds.extents.y + originOffsetDist);

                    raycastDir = Vector2.left * facingDir;

                    raycastDistance = playerCollider.bounds.size.x;

                    RaycastHit2D seccondHit2D = Physics2D.Raycast(raycastOrigin, raycastDir, raycastDistance, groundLayer);
                    if (seccondHit2D.distance > playerCollider.bounds.size.x * maxEmptyHeadWidthToNudge)
                    {
                        if (!Physics2D.Raycast(seccondHit2D.point + (raycastDir * -0.005f), raycastDir * -1, playerCollider.bounds.size.x, groundLayer))
                        {
                            transform.position = new Vector3(seccondHit2D.point.x + ((playerCollider.bounds.extents.x + 0.01f) * facingDir), transform.position.y);
                            teleported = true;
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Wall Slide + Wall Jump
    public void wallSlide()
    {
        // Check if: WallCheck is true, not Grounded, and is falling
        bool isWallSliding = WallCheck && !GroundCheck && Rb.linearVelocityY < -0.01f;

        if (hasWallSlide && isWallSliding)
        {
            Rb.gravityScale = originalGravScale * wallSlideFac;

            if (Input.GetButtonDown("Jump") && hasWallJump)
            {
                int facingDir = isFacingRight ? 1 : -1;

                performJump(); // Apply vertical jump force
                // Apply horizontal push away from the wall
                AddHorizontalForce(wallJumpPush, -facingDir, wallJumpDur, ForceMode2D.Impulse);
            }
        }
    }
    #endregion

    #region Dash

    private void dash()
    {
        if (hasDash && Input.GetMouseButtonDown(1) && canDash)
        {
            // Ensure only one dash coroutine is running
            if (DashCoroutine != null) StopCoroutine(DashCoroutine);
            DashCoroutine = StartCoroutine(StartDashCoroutine());
        }
    }

    IEnumerator StartDashCoroutine()
    {
        isDashing = true;
        canDash = false;

        int facingDir = isFacingRight ? 1 : -1;

        // Apply horizontal dash force
        AddHorizontalForce(dashForce, facingDir, dashDur, ForceMode2D.Impulse);

        // Wait for the horizontal force effect to complete
        yield return new WaitUntil(() => isHorizCouRunning == false);

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    #endregion

    #region Utils
    public void SpeedMultplier()
    {

    }

    public void ResetToDefaultSpeed()
    {
        currentMoveSpeed = PlayerSpeed;
    }

    #region AddHorizontalForce (Utility for Dash/Wall Jump)

    public void AddHorizontalForce(float force, int dir, float duration, ForceMode2D forceMode, bool isNoGravity = false)
    {
        if (isHorizCouRunning)
        {
            StopCoroutine(horizForceCou);
        }

        horizForceCou = StartCoroutine(AddSidewayForceCoroutine(force, dir, duration, forceMode, isNoGravity));
    }

    IEnumerator AddSidewayForceCoroutine(float force, int dir, float duration, ForceMode2D forceMode, bool isNoGravity)
    {
        isHorizCouRunning = true;

        float tempGravScale = Rb.gravityScale;

        if (isNoGravity)
        {
            Rb.gravityScale = 0;
        }

        Rb.AddForce(Vector2.right * dir * force, forceMode);
        canMove = false;

        yield return new WaitForSeconds(duration);

        // Stop horizontal motion abruptly
        Rb.linearVelocityX = 0;
        canMove = true;

        if (isNoGravity)
        {
            Rb.gravityScale = tempGravScale;
        }
        isHorizCouRunning = false;
    }

    #endregion

    #endregion

    // The OnDrawGizmos region from your original code remains valid for visualization
    // and is omitted here for brevity, assuming you are not asking for a refactor of the Gizmos.

}