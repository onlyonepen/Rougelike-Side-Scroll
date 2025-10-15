using System.Collections;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{

    public float PlayerSpeed = 10f;
    public float JumpForce = 7f;
    public bool CanDoubleJump = false;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Collider2D playerCollider;

    //[Header("floating capsule")]
    //[SerializeField] private float floatingDis = 0.5f;
    //[SerializeField] private float springStrengh = 5;
    //[SerializeField] private float RideSpringDamper = 3f;

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


    [Header("Coyote, jump buffering")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferingTime = 0.2f;
    float coyoteJumpTimer = 0;
    float bufferJumpTimer = 0;

    [Header("Varible jump height")]
    [Tooltip("If true will use velocity cut method but if false will increase gravity on release jump")]
    [SerializeField] private bool jumpHeightCut = true;
    [SerializeField] private float gravMultJump = 3;

    [Header("Faster Falling")]
    [SerializeField] private bool hasFasterFall = true;
    [SerializeField] private float fasterFallMult = 1.5f;

    [Header("Apex Modifier")]
    [SerializeField] private bool hasApexModifier = true;
    [SerializeField] private float velocityDisplaceForApexBonus = 1;
    [SerializeField] private float apexSpeedMult = 1.4f;
    private float apexSpeed;
    [SerializeField] private float apexHoverGravMult = 0.5f;
    private float apexGrav;

    [Header("Max Fall Speed")]
    [SerializeField] private bool hasMaxFallSpeed = true;
    [SerializeField] private float maxFallSpeed = 15;

    [Header("EdgeNudge")]
    [SerializeField] private bool hasWallEdgeNudge = true;
    [SerializeField] private float maxEmptyHeadWidthToNudge = 2/3;

    [Header("Seralize")]
    [SerializeField] private PlayerManagerScript playerManager;
    //[SerializeField] private bool hasRoofEdgeNudge = true;


    [HideInInspector]
    public bool WallCheck;
    [HideInInspector]
    public bool GroundCheck;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public bool isFacingRight = true;
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public Coroutine DashCoroutine;
    [HideInInspector]
    public bool canDash = true;

    float speedTemp;
    float originalGrav;
    bool isHorizCouRunning = false;
    Coroutine horizForceCou;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speedTemp = PlayerSpeed;
        apexSpeed = PlayerSpeed * apexSpeedMult;
        originalGrav = rb.gravityScale;
        apexGrav = rb.gravityScale * apexHoverGravMult;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
        }
            horizontalMovement();
            jumpLogic();
            wallSlide();
            dash();
            Flip();

        varibleJumpHeight();
        apexModifier();
        fasterFall();
        clampFallSpeed();
        edgeNudge();
    }

    #region Unused
    //private void floatingCapsule()
    //{
    //    Vector2 rayOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
    //    RaycastHit2D floatingRay = Physics2D.Raycast(rayOrigin, Vector2.down, floatingDis, groundLayer);
    //    if (floatingRay)
    //    {
    //        float playerVelY = rb.linearVelocity.y;
    //        float otherVelY = 0f;
    //        if (floatingRay.rigidbody != null)
    //        {
    //            otherVelY = floatingRay.rigidbody.linearVelocity.y;
    //        }
    //        float relVel = playerVelY - otherVelY;
    //        float x = floatingDis - floatingRay.distance;
    //        float springForce = (x * springStrengh) - (relVel * RideSpringDamper);

    //        if (!Input.GetButton("Jump"))
    //        {
    //            rb.AddForce(Vector2.up * springForce, ForceMode2D.Force);
    //        }
    //        else if (GroundCheck && rb.linearVelocityY < 0)
    //        {
    //            rb.AddForce(Vector2.up * springForce, ForceMode2D.Force);
    //        }
    //    }
    //}

    //private void horizontalMovement()
    //{
    //    if (!isDashing)
    //    {
    //        float horizontalInput = Input.GetAxisRaw("Horizontal");

    //        Vector2 BoxcastOrigin = playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x * horizontalInput, 0);
    //        Vector2 BoxcastSize = new Vector2(0.05f, playerCollider.bounds.size.y);
    //        WallCheck = Physics2D.BoxCast(BoxcastOrigin, BoxcastSize, 0, Vector2.zero, Mathf.Infinity, groundLayer);

    //        if (!WallCheck)
    //        {
    //            rb.AddForce(new Vector2(horizontalInput * speedTemp, 0));
    //            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocity.x, -speedTemp, speedTemp);
    //        }

    //        if(horizontalInput == 0 /*&& Mathf.Abs(rb.linearVelocity.x) < 0.5f*/)
    //        {
    //            rb.linearVelocity = new Vector3(0, rb.linearVelocityY);
    //        }
    //    }
    //}
    #endregion

    private void horizontalMovement()
    {
        if (canMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float horizontalInputRaw = Input.GetAxisRaw("Horizontal");

            Vector2 BoxcastOrigin = playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x * horizontalInputRaw, 0);
            Vector2 BoxcastSize = new Vector2(0.05f, playerCollider.bounds.size.y);
            WallCheck = Physics2D.BoxCast(BoxcastOrigin, BoxcastSize, 0, Vector2.zero, Mathf.Infinity, groundLayer);

            if (horizontalInput != 0 && !WallCheck)
            {
                transform.position += new Vector3(horizontalInput * speedTemp * Time.deltaTime, 0);
            }
        }
    }

    #region Jump Logic
    private void jumpLogic()
    {
        Vector2 BoxcastOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 BoxcastSize = new Vector2(playerCollider.bounds.size.x * 0.95f ,0.1f);
        GroundCheck = Physics2D.BoxCast(BoxcastOrigin, BoxcastSize, 0, Vector2.zero, Mathf.Infinity, groundLayer);
        if(GroundCheck)
        {
            rb.gravityScale = originalGrav;
        }

        #region Coyote and buffering
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
        #endregion

        if (coyoteJumpTimer > 0 && bufferJumpTimer > 0)
        {
            playerManager.CancelDrawCard();
            if (canMove)
            {
                performJump();
            }
            coyoteJumpTimer = 0;
            bufferJumpTimer = 0;
        }
    }

    private void performJump()
    {
        rb.linearVelocityY = 0;
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    #endregion

    #region Varible jump height
    private void varibleJumpHeight()
    {
        if (jumpHeightCut)
        {
            if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); //varible jump height
                //coyoteJumpTimer = 0;
            }
        }
        else
        {
            if (Input.GetButtonUp("Jump") /*&& rb.linearVelocityY > 0*/)
            {
                rb.gravityScale = originalGrav * gravMultJump;
            }
        }
    }
    #endregion

    #region Apex modifier
    private void apexModifier()
    {
        if (hasApexModifier)
        {
            if (Input.GetButton("Jump") && Mathf.Abs(rb.linearVelocityY) < velocityDisplaceForApexBonus)
            {
                speedTemp = apexSpeed;
                rb.gravityScale = apexGrav;
            }
            else
            {
                speedTemp = PlayerSpeed; // Must check when editing speed
                //rb.gravityScale = gravTemp;
            }
        }
    }
    #endregion

    #region Faster fall
    private void fasterFall()
    {
        if (hasFasterFall)
        {
            if(rb.linearVelocityY < -velocityDisplaceForApexBonus && rb.gravityScale <= originalGrav)
            {
                rb.gravityScale = originalGrav * fasterFallMult;
            }
        }
    }
    #endregion

    #region Clamp fall speed
    private void clampFallSpeed()
    {
        if (hasMaxFallSpeed)
        {
            if(rb.linearVelocityY < -maxFallSpeed)
            {
                rb.linearVelocityY = -maxFallSpeed;
            }
        }
    }
    #endregion

    #region Edge nudge
    
    private void edgeNudge()
    {
        if (hasWallEdgeNudge)
        {
            if (rb.linearVelocityY > 0) //assending
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
        if (hasWallSlide)
        {
            float horizontalInputRaw = Input.GetAxisRaw("Horizontal");
            if(/*horizontalInputRaw != 0 && */WallCheck && !GroundCheck && rb.linearVelocity.y < -0.01f) //is moving & is at wall & not on ground & is falling
            {
                rb.gravityScale = originalGrav * wallSlideFac;

                if (Input.GetButtonDown("Jump") && hasWallJump)
                {
                    int facingDir;
                    if (isFacingRight) facingDir = 1;
                    else facingDir = -1;

                    playerManager.CancelDrawCard();
                    if (canMove)
                    {
                        performJump();
                        AddHorizontalForce(wallJumpPush, -facingDir, wallJumpDur, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
    #endregion

    #region Dash

    private void dash()
    {
        if (hasDash && Input.GetMouseButtonDown(1) && canDash)
        {
            playerManager.CancelDrawCard();
            if (canMove)
            {
                DashCoroutine = StartCoroutine(startDash());
            }
        }
    }

    IEnumerator startDash()
    {
        isDashing = true;
        canDash = false;

        int facingDir;
        if (isFacingRight) facingDir = 1;
        else facingDir = -1;

        Debug.Log("dash");
        AddHorizontalForce(dashForce, facingDir, dashDur, ForceMode2D.Impulse);

        yield return new WaitUntil(() => (isHorizCouRunning == false));

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    #endregion

    #region Flip
    private void Flip()
    {
        if(!isDashing && canMove)
        {
            if (isFacingRight && Input.GetAxisRaw("Horizontal") < 0f || !isFacingRight && Input.GetAxisRaw("Horizontal") > 0f)
            {
                Vector3 localScale = transform.localScale;
                isFacingRight = !isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }
    #endregion


    public void SpeedMultplier()
    {
        
    }
    
    public void ResetToDefaultSpeed()
    {
        speedTemp = PlayerSpeed;
    }

    #region Utils

    #region AddHorizontalForce

    public void AddHorizontalForce(float force, int dir, float duration, ForceMode2D forceMode, bool isNoGravity = false)
    {
        if (isHorizCouRunning)
        {
            StopCoroutine(horizForceCou);
        }

        horizForceCou = StartCoroutine(addSidewayForce(force, dir, duration, forceMode, isNoGravity));
    }

    IEnumerator addSidewayForce(float force, int dir, float duration, ForceMode2D forceMode, bool isNoGravity)
    {
        isHorizCouRunning = true;
        if(isNoGravity)
        {
            //float originalGravity = rb.gravityScale;
            rb.gravityScale = 0;
        }

        rb.AddForce(Vector2.right * dir * force, forceMode);
        canMove = false;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        canMove = true;

        if (isNoGravity)
        {
            rb.gravityScale = originalGrav;
        }
        isHorizCouRunning = false;
    }

    #endregion

    #endregion

    private void OnDrawGizmos()
    {
        if (playerCollider == null) return;

        // Visualize the logic only if the player is ascending
        if (rb.linearVelocity.y > 0)
        {
            // First Raycast (in the direction the player is facing)
            VisualizeRaycast(isFacingRight ? 1 : -1, "Primary", Color.yellow, Color.red, Color.blue);

            // Second Raycast (only if the first one fails)
            // Note: The Gizmo logic mirrors the script, so it will check both unless the first one is successful.
            VisualizeRaycast(isFacingRight ? -1 : 1, "Secondary", Color.cyan, Color.magenta, Color.green);
        }
    }

    private void VisualizeRaycast(int facingDir, string name, Color rayColor, Color hitColor, Color lineToHitColor)
    {
        Vector2 raycastOrigin = playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x * facingDir, playerCollider.bounds.extents.y + 0.2f);
        Vector2 raycastDir = Vector2.left * facingDir;
        float raycastDistance = playerCollider.bounds.size.x;

        // Draw the main raycast line
        Gizmos.color = rayColor;
        Gizmos.DrawRay(raycastOrigin, raycastDir * raycastDistance);

        RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, raycastDir, raycastDistance, groundLayer);

        if (hit2D)
        {
            // Draw a sphere at the hit point
            Gizmos.color = hitColor;
            Gizmos.DrawSphere(hit2D.point, 0.1f);

            // Draw a line from the origin to the hit point
            Gizmos.color = lineToHitColor;
            Gizmos.DrawLine(raycastOrigin, hit2D.point);

            // Now, visualize the second, "safety" raycast
            Vector2 safetyRayOrigin = hit2D.point + (raycastDir * -0.005f);
            Vector2 safetyRayDir = raycastDir * -1;
            float safetyRayDistance = playerCollider.bounds.size.x;

            //Gizmos.color = Color.white;
            //Gizmos.DrawRay(safetyRayOrigin, safetyRayDir * safetyRayDistance);

            // Check the conditions for teleporting
            if (hit2D.distance > playerCollider.bounds.size.x * maxEmptyHeadWidthToNudge)
            {
                if (!Physics2D.Raycast(safetyRayOrigin, safetyRayDir, safetyRayDistance, groundLayer))
                {
                    // Draw a larger green sphere when both conditions for teleportation are met
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(hit2D.point, 0.2f);
                }
            }
        }
    }
}