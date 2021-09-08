using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Framework.AMVC;
using UnityEditor;

public class PlayerController : View<GameplayApp>
{
    #region Variables

    [HideInInspector]
    public Rigidbody2D rb;

    Collision col;
    CapsuleCollider2D boxCollider;

    public PhysicsMaterial2D noFriction, maxFriction;

    private float moveInput;
    private float gravityScale;
    private int wallSide;
    private Vector2 colliderSize;

    public float inputBufferCounter;
    public float inputBufferMax = 4;

    public float coyoteTimer;
    public float coyoteFrames = 4;

    public Vector2 approachDir;
    public float xRaw;
    public float yRaw;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    [SerializeField]
    private float fallingSpeedLimit;


    [Space]
    [Header("Slope Check")]

    [SerializeField]
    private float slopeCheckDistance;
    [SerializeField]
    private float maxSlopeAngle = 70f;

    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;

    private Vector2 slopeNormalPerp;

    public float maxRotateAngle = 6.5f;


    [Space]
    [Header("Stats")]
    public float moveSpeed;
    public float planetMoveSpeed;
    public float jumpForce;
    public float planetJumpForce;
    public float dashSpeed;
    public float dashTime;
    public float wallSlideSpeed;
    public float wallStickTime = 0.25f;
    public float wallStickTimer;

    [Space]
    [Header("Booleans")]
    public bool facingRight = true;
    public bool groundTouch;
    public bool isMoving;
    public bool canMove;
    public bool canDash;
    public bool canJump;
    public bool canWallJump;
    public bool canWalkOnSlope;
    public bool isJumping;
    public bool isWallSliding;
    public bool isOnSlope;
    public bool downHill;
    public bool canFlip;
    public bool isOnPlanet;

    [Space]
    public bool hasJumped;
    public bool hasDoubleJumped;


    [Space]
    public bool isDashing;
    public bool hasDashed;
    public bool isReflectDashing;
    public bool hasReflected;


    [Space]
    [Header("Polish")]
    [SerializeField] private ParticleSystem dashParticle;
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private ParticleSystem wallJumpParticle;
    [SerializeField] private ParticleSystem slideParticle;
    [SerializeField] private SpriteRenderer visual;

    private Animator anim;

    [HideInInspector]
    public Vector3 planet;
    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collision>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        colliderSize = boxCollider.size;
    }

    private void Start()
    {
        canMove = true;
        gravityScale = rb.gravityScale;
        inputBufferCounter = inputBufferMax;
    }

    private void Update()
    {
        CheckInput();
        CheckIfCanJump();
        CheckGroundTouch();
        CheckDirection();
        CheckWallSlide();
        SlopeCheck();
        if (!isOnPlanet)
        {
            transform.up = Vector3.MoveTowards(transform.up, Vector3.up, rb.gravityScale * Time.deltaTime * 1.5f);
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void LateUpdate()
    {
        UpdateAnimations();
        RotatePlayer();
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("onGround", col.onGround);
        //anim.SetBool("isDashing", isDashing);
        //anim.SetBool("isWallSliding", isWallSliding);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(facingRight ? 1 : -1, transform.localScale.y, transform.localScale.z);
    }

    private void ApplyMovement()
    {
        if (!canMove)
            return;

        if (isWallSliding && wallStickTimer > 0)
            return;

        if (isOnPlanet && planet != null && canMove)
        {
            if (col.onGround)
                transform.RotateAround(planet, Vector3.forward, -moveInput * planetMoveSpeed * Time.fixedDeltaTime);
            else
                transform.RotateAround(planet, Vector3.forward, -moveInput * planetMoveSpeed / 2f * Time.fixedDeltaTime);
        }
        else
        {
            if ((col.onGround && !isOnSlope) || !col.onGround) //On Ground Or In Air
            {
                rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            }
            else if (col.onGround && isOnSlope && !isJumping && canWalkOnSlope && !col.isTouchingWall) // On Slope
            {
                rb.velocity = new Vector2(moveSpeed * slopeNormalPerp.x * -moveInput, moveSpeed * slopeNormalPerp.y * -moveInput);
            }

            //Clamp Falling Speed
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Mathf.Abs(fallingSpeedLimit)));
        }

        // rb.velocity = new Vector2(moveInput * moveSpeed, Mathf.Max(rb.velocity.y, -Mathf.Abs(fallingSpeedLimit)));
    }

    public void FreezePlayer(bool isFrozen)
    {
        if(isFrozen)
        {
            rb.sharedMaterial = maxFriction;
            moveInput = 0;
        }
        rb.velocity = Vector2.zero;
        canMove = !isFrozen;
    }

    #region Slope Check

    void SlopeCheck()
    {
        if (!canMove)
            return;
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2f);

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, col.Ground);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, col.Ground);

        Debug.DrawRay(checkPos, transform.right * slopeCheckDistance, Color.red);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }

    }

    void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, col.Ground); //cast downward ray from the bottom of the player 

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            downHill = Vector2.Angle(hit.normal, Vector2.left) <= 90f ? false : true;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up); //The angle of the slope = the angle between the y-axis and the slope normal

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }
            else if (slopeDownAngle == 0.0f)
            {
                isOnSlope = false;
            }

            lastSlopeAngle = slopeDownAngle;
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && moveInput == 0 && canWalkOnSlope)
        {
            rb.sharedMaterial = maxFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }


    //Slightly Rotate Sprite When On Slope
    void RotatePlayer()
    {
        float rotateAngle;
        if (isOnSlope && !isJumping && isMoving && !isOnPlanet)
        {
            rotateAngle = Mathf.Clamp(slopeDownAngle / 3.5f, 0, maxRotateAngle);
            rotateAngle = downHill ? -rotateAngle : rotateAngle;

            visual.gameObject.transform.DORotate(new Vector3(0, 0, rotateAngle), 0.2f);
            //sr.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, downHill ? -slopeDownAngle / 3f : slopeDownAngle / 3f));
        }
        else
        {
            rotateAngle = transform.eulerAngles.z;
            visual.gameObject.transform.DORotate(new Vector3(0, 0, rotateAngle), 0.0f);
        }
    }

    #endregion

    #region Check Conditions Functions
    private void CheckInput()
    {
        moveInput = Input.GetAxis("Horizontal");

        //Implement coyote time: for a few frame after leaving the ground, player can still jump
        if (!col.onGround && coyoteTimer < coyoteFrames)
            coyoteTimer += Time.deltaTime;

        //Check if can jump and use jump/input buffer
        //Jump buffer mean when the player press the jump button a few frame early
        //...the jump will still happen when hit the ground
        if (Input.GetButtonDown("Jump"))
        {
            inputBufferCounter = 0;
        }
        if (inputBufferCounter < inputBufferMax)
        {
            inputBufferCounter += Time.deltaTime;

            if (coyoteTimer < coyoteFrames && !hasJumped)
            {
                if (isOnPlanet)                   
                    PlanetJump();
                else
                    Jump(Vector2.up);
            }
        }
        //Check Wall Jump
        if (col.isTouchingWall && !col.onGround && Input.GetButtonDown("Jump"))
        {
            //AudioManager.instance.PlayJumpSFX("Jump", 0.2f);
            // AppRoot.Instance.GetService<SfxController>().Play("fx-jump");
            WallJump();
        }

        //Check Dash
        if (Input.GetButtonDown("Dash"))
        {
            Debug.LogWarning("Dash button");

            xRaw = Input.GetAxisRaw("Horizontal");
            yRaw = Input.GetAxisRaw("Vertical");

            //Dash forward if no key were pressed
            if (xRaw == 0 && yRaw == 0)
                Dash(facingRight ? 1 : -1, 0);
            else
                Dash(xRaw, yRaw);
        }
    }

    private void CheckDirection()
    {
        if (moveInput < 0 && facingRight && !isWallSliding)
        {
            Flip();

        }
        else if (moveInput > 0 && !facingRight && !isWallSliding)
        {
            Flip();
        }

        if (!isOnPlanet)
            isMoving = Mathf.Abs(rb.velocity.x) >= 0.1f ? true : false;
        else
            isMoving = Mathf.Abs(moveInput) > 0;
    }

    private void CheckIfCanJump()
    {
        if (!col.onGround)
            isOnSlope = false;

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (col.onGround && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

        //Check Double Jump
        //if (!col.onGround && !col.isTouchingWall && !hasDoubleJumped && coyoteTimer >= coyoteFrames)
        //{
        //    canDoubleJump = true;
        //}

        //-------------------------Check wall jump-----------------------------------

        if (col.isTouchingWall)
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }


    }

    private void CheckWallSlide()
    {
        wallSide = col.wallSide;
        if (col.isTouchingWall && !col.onGround && rb.velocity.y <= 0 && moveInput != 0)
        {
            if (moveInput == wallSide || wallStickTimer > 0)
            {
                isWallSliding = true;
                WallSlide();

                //Implement sticky wall
                if (moveInput != wallSide && moveInput != 0)
                {
                    wallStickTimer -= Time.deltaTime;
                }
                else
                {
                    wallStickTimer = wallStickTime;
                }
            }
        }

        if (!col.isTouchingWall || col.onGround)
        {
            isWallSliding = false;
        }
    }

    private void CheckGroundTouch()
    {
        if (col.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!col.onGround && groundTouch)
        {
            groundTouch = false;
        }
    }

    private void GroundTouch()
    {
        canJump = true;
        //canDoubleJump = false;
        isJumping = false;
        hasJumped = false;
        hasDoubleJumped = false;
        hasDashed = false;
        hasReflected = false;
        coyoteTimer = 0; //Reset coyote timer
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.enabled)
            return;

        if (isDashing && collision.gameObject.layer == LayerMask.NameToLayer("Reflect Platform"))
        {
            //Check if reach minimum velocity before reflect
            if (!hasReflected)
            {
                if (xRaw == 0 && yRaw == 0)
                    Reflect(collision, new Vector2(facingRight ? 1 : -1, 0));
                else
                    Reflect(collision, new Vector2(xRaw, yRaw));
            }
            else
                Reflect(collision, approachDir);

            hasReflected = true;
        }
    }
    #endregion

    #region Movement Function
    public void DisableMovement(float time)
    {
        StartCoroutine(IDisableMovement(time));
    }

    private void Jump(Vector2 dir)
    {
        if (!canMove)
            return;

        if (canJump)
        {
            Debug.Log("Jump");
            canJump = false;
            isJumping = true;
            hasJumped = true;
            inputBufferCounter = inputBufferMax;
            coyoteTimer = coyoteFrames;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += dir * jumpForce;

        }
    }

    private void PlanetJump()
    {
        //AudioManager.instance.PlayJumpSFX("Jump", 0.5f);
        canJump = false;
        isJumping = true;
        hasJumped = true;
        inputBufferCounter = inputBufferMax;
        coyoteTimer = coyoteFrames;
        rb.velocity = Vector2.zero;
        Vector3 dir = (planet - transform.position).normalized;
        rb.AddForce(-dir * planetJumpForce, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        rb.velocity = Vector2.zero;

        if (canWallJump)
        {
            Debug.Log("Wall Jump");
            StopCoroutine(IDisableMovement(0f));
            StartCoroutine(IDisableMovement(0.1f));

            isWallSliding = false;
            //hasDoubleJumped = false;

            //Wall Jump Climb
            if (wallSide == moveInput)
            {
                rb.velocity = new Vector2(-wallSide * wallJumpClimb.x, wallJumpClimb.y);
            }
            //Wall Jump Off
            //else if (moveInput == 0)
            //{
            //    rb.velocity = new Vector2(-wallSide * wallJumpOff.x, wallJumpOff.y);
            //}
            //Wall Leap
            else
            {
                rb.velocity = new Vector2(-wallSide * wallLeap.x, wallLeap.y);
            }
        }
    }

    private void WallSlide()
    {
        if (isWallSliding)
        {
            bool pushingWall = false;

            if (rb.velocity.x > 0 && col.isTouchingWall)
            {
                pushingWall = true;
            }

            float push = pushingWall ? 0 : rb.velocity.x;
            rb.velocity = new Vector2(push, -wallSlideSpeed);
        }
    }

    private void Dash(float x, float y)
    {
        if (hasDashed || isDashing)
            return;
        hasDashed = true;
        //AudioManager.instance.PlaySFX("Dash", 0.3f);
        //sr.material = dashMaterial;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity = dir.normalized * dashSpeed;
        StopCoroutine(DashWait());
        StartCoroutine(DashWait());
    }

    private void Reflect(Collision2D mirror, Vector2 dir)
    {
        //AudioManager.instance.PlaySFX("Shatter", 1f);
        rb.velocity = Vector2.zero;
        Vector2 reflectDir = Vector2.Reflect(dir, mirror.GetContact(0).normal);
        approachDir = new Vector2(Mathf.Round(reflectDir.x), Mathf.Round(reflectDir.y));

        rb.velocity = reflectDir.normalized * dashSpeed;
        canDash = true;
        hasDashed = false;
        //canDoubleJump = true;
        hasDoubleJumped = false;

        StopCoroutine(DashWait());
        StartCoroutine(ReflectDashWait());
    }

    private void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    public void UnlockDash()
    {
        canDash = true;
    }

    private IEnumerator DashWait()
    {
        //FindObjectOfType<GhostTrail>().ShowGhost();
        DOVirtual.Float(12.5f, 0, 0.8f, RigidbodyDrag);
        rb.gravityScale = 0;
        canMove = false;
        isDashing = true;
        if (dashParticle != null) dashParticle.Play();

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravityScale;
        canMove = true;
        isDashing = false;
        hasDashed = !col.onGround;
        hasReflected = false;
        if (dashParticle != null) dashParticle.Stop();
        //trail.SetActive(false);
    }

    private IEnumerator ReflectDashWait()
    {
        //FindObjectOfType<GhostTrail>().ShowGhost();
        DOVirtual.Float(12.5f, 0, 0.8f, RigidbodyDrag);
        rb.gravityScale = 0;
        canMove = false;
        isReflectDashing = true;
        hasDashed = false;
        if (dashParticle != null) dashParticle.Play();
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravityScale;
        canMove = true;
        hasDashed = false;
        isReflectDashing = false;
        hasReflected = false;
        if (dashParticle != null) dashParticle.Stop();
    }

    private IEnumerator IDisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            //AudioManager.instance.PlaySFX("Attract", 0.25f);
            planet = collision.transform.position;
            rb.velocity = new Vector2(rb.velocity.x / 2, rb.velocity.y / 2);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            isOnPlanet = true;
            rb.gravityScale = 0f;
            rb.drag = 5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            isOnPlanet = false;
            rb.gravityScale = 5f;
            rb.drag = 0;
        }
    }
}
