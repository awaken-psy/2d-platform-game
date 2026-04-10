using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    private bool canbecontrolled = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private float defaultGravityScale;
    private bool canDoubleJump;

    [Header("Buffer Jump & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow;

    [SerializeField] private float coyoteJumpWindow;
    private float bufferJumpActivate = -1;
    private float coyoteJumpActivate = -1;

    [Header("Wall interaction")]
    [SerializeField] private float wallJumpDuration;

    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration;

    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked = false;

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;

    [SerializeField] private float wallCheckDistance;

    [SerializeField] private LayerMask WhatIsGround;

    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private float xInput;
    private float yInput;

    private bool facingRight = true;
    private int faceDir = 1;

    [Header("VFX")]
    [SerializeField] private GameObject DeathVFX;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start() {
        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);
    }

    private void Update() {
        UpdateAirbornStatus();

        if (!canbecontrolled) {
            HandleCollision();
            HandleAnimations();
            return;
        }
        if (isKnocked)
            return;

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    #region respawn

    public void RespawnFinished(bool finished) {
        if (finished) {
            canbecontrolled = true;
            rb.gravityScale = defaultGravityScale;
            cd.enabled = true;
        }
        else {
            canbecontrolled = false;
            rb.gravityScale = 0;
            cd.enabled = false;
        }
    }

    #endregion respawn

    #region Knockback

    public void Knockback(float sourceDamageXPosition) {
        float knockbackDir = 1;
        if (transform.position.x < sourceDamageXPosition)
            knockbackDir = -1;

        if (isKnocked)
            return;
        StartCoroutine(KnockbackRoutine());
        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine() {
        isKnocked = true;
        anim.SetBool("isKnocked", isKnocked);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        anim.SetBool("isKnocked", isKnocked);
    }

    #endregion Knockback

    #region die

    public void Die() {
        Destroy(gameObject);
        GameObject newDeathVFX = Instantiate(DeathVFX, transform.position, Quaternion.identity);
    }

    #endregion die

    #region push

    public void Push(Vector2 dir, float duration = 0) {
        StartCoroutine(PushCouroutine(dir, duration));
    }

    private IEnumerator PushCouroutine(Vector2 dir, float duration) {
        canbecontrolled = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        canbecontrolled = true;
    }

    #endregion push

    #region airborne and landing

    private void UpdateAirbornStatus() {
        if (isAirborne && isGrounded) {
            HandleLanding();
        }
        else if (!isAirborne && !isGrounded) {
            BecomeAirborne();
        }
    }

    private void BecomeAirborne() {
        isAirborne = true;

        if (rb.velocity.y < 0)
            ActivateCoyoteJump();
    }

    private void HandleLanding() {
        isAirborne = false;
        canDoubleJump = true;
        AttemptBufferJump();
    }

    private void HandleInput() {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) {
            JumpButton();
            RequestBufferJump();
        }
    }

    #endregion airborne and landing

    #region Jumping

    private void JumpButton() {
        bool canCoyoteJump = Time.time < coyoteJumpActivate + coyoteJumpWindow;
        if (isGrounded || canCoyoteJump) {
            Jump();
        }
        else if (isWallDetected && !isGrounded) {
            WallJump();
        }
        else if (canDoubleJump) {
            DoubleJump();
        }
        CancelCoyoteJump();
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void DoubleJump() {
        isWallJumping = false;
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    #region Buffer Jump & Coyote Jump

    private void RequestBufferJump() {
        if (isAirborne)
            bufferJumpActivate = Time.time;  // 表示自游戏开始以来经过的总时间（以秒为单位）
    }

    private void AttemptBufferJump() {
        if (Time.time < bufferJumpActivate + bufferJumpWindow) {
            Jump();
            bufferJumpActivate = Time.time - 1; // 使其过期
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivate = Time.time;

    private void CancelCoyoteJump() => coyoteJumpActivate = Time.time - 1;

    #endregion Buffer Jump & Coyote Jump

    #region Wall Jump

    private void WallJump() {
        canDoubleJump = true;
        rb.velocity = new Vector2(wallJumpForce.x * -faceDir, wallJumpForce.y);
        Flip();
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine() {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    #endregion Wall Jump

    #endregion Jumping

    #region handle

    private void HandleWallSlide() {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yModifier = yInput < 0 ? 1 : .05f;
        if (canWallSlide == false)
            return;

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifier);
    }

    private void HandleCollision() {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, WhatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * faceDir, wallCheckDistance, WhatIsGround);
    }

    private void HandleAnimations() {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement() {
        if (isWallDetected)
            return;
        if (isWallJumping)
            return;
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleFlip() {
        if (facingRight && xInput < 0 || !facingRight && xInput > 0)
            Flip();
    }

    private void Flip() {
        faceDir *= -1;
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    #endregion handle

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * faceDir * wallCheckDistance);
    }
}