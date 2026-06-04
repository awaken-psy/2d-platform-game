using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 玩家主控制器，负责移动、跳跃（含二段跳/墙跳/土狼跳/缓冲跳）、碰撞检测、击退、死亡与重生。
/// 使用 Raycast 进行地面与墙壁检测，通过 Animator 参数驱动动画状态机。
/// </summary>
public class player : MonoBehaviour
{
    private Difficulty gameDifficulty;
    private GameManager gameManager;

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
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsEnemy;

    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private float xInput;
    private float yInput;

    private bool facingRight = true;
    private int faceDir = 1;

    [Header("Player Visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject DeathVFX;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start() {
        gameManager = GameManager.instance;
        UpdateDifficulty();

        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);

        ChooseSkin();
    }

    private void Update() {
        UpdateAirbornStatus();

        // 重生动画期间：仅保留碰撞检测与动画更新，不接受输入
        if (!canbecontrolled) {
            HandleCollision();
            HandleAnimations();
            return;
        }
        // 被击退期间：暂停玩家输入处理，等待协程结束
        if (isKnocked) {
            return;
        }

        HandleEnemyDetection();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();
    }

    #region difficulty
    public void Damage() {
        if (gameDifficulty == Difficulty.Hard) {
            Die();
            gameManager.RestartLevel();
        }
        else if (gameDifficulty == Difficulty.Normal) {
            gameManager.RemoveFruit();
            if (gameManager.FruitCollected() < 0) {
                Die();
                gameManager.RestartLevel();
            }
        }
        else {
            ;
        }
    }
    private void UpdateDifficulty() {
        DifficultyManager dm = DifficultyManager.instance;
        if (dm != null)
            gameDifficulty = dm.difficulty;
    }
    #endregion difficulty
    #region skin
    public void ChooseSkin() {
        anim.runtimeAnimatorController = animators[SkinManager.instance.getSkinId()];
    }
    #endregion skin

    #region enemy
    /// <summary>
    /// 踩敌人检测：当玩家下落速度向下时，检测 enemyCheck 范围内的敌人并触发其死亡，同时让玩家再次跳跃。
    /// </summary>
    private void HandleEnemyDetection() {
        if (rb.velocity.y >= 0) {
            return;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);
        foreach (Collider2D enemy in colliders) {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            if (newEnemy != null) {
                newEnemy.Die();
                Jump();
            }
        }
    }
    #endregion enemy

    #region respawn

    /// <summary>
    /// 控制重生状态：finished 为 true 时恢复重力与碰撞，允许输入；false 时冻结玩家用于播放入场动画。
    /// 由 PlayerAnimationEvents.FinishRespawn 在重生动画结束时调用。
    /// </summary>
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

    /// <summary>
    /// 触发击退：根据伤害来源的 X 坐标决定击退方向，短暂剥夺控制权。
    /// </summary>
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

    /// <summary>
    /// 外部推力接口（如蹦床）：短暂禁用控制并施加瞬时冲量。
    /// </summary>
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

        // 离开地面且开始下落时激活土狼跳窗口，允许玩家在刚离开地面瞬间仍能起跳
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

    /// <summary>
    /// 跳跃优先级：地面/土狼跳 > 墙跳 > 二段跳。
    /// 一旦起跳，立即取消土狼跳窗口，防止重复触发。
    /// </summary>
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

    /// <summary>
    /// 缓冲跳：若玩家在空中按下跳跃键，记录时间点；落地后若仍在缓冲窗口内则自动起跳。
    /// </summary>
    private void RequestBufferJump() {
        if (isAirborne)
            bufferJumpActivate = Time.time;
    }

    private void AttemptBufferJump() {
        if (Time.time < bufferJumpActivate + bufferJumpWindow) {
            Jump();
            bufferJumpActivate = Time.time - 1; // 使缓冲过期，防止连续触发
        }
    }

    /// <summary>
    /// 土狼跳（Coyote Jump）：玩家离开地面开始下落后的一小段时间内仍可起跳，提升手感宽容度。
    /// </summary>
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

    /// <summary>
    /// 滑墙：贴墙下落时大幅降低 Y 轴速度；按住下方向键时加速下滑。
    /// </summary>
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

    /// <summary>
    /// 翻转角色朝向：使用 Rotate(0,180,0) 而非缩放，避免碰撞体和子对象异常。
    /// </summary>
    private void Flip() {
        faceDir *= -1;
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    #endregion handle

    private void OnDrawGizmos() {
#if UNITY_EDITOR
        Handles.color = Color.black;
        Handles.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance, 3f);
        Handles.DrawLine(transform.position, transform.position + Vector3.right * faceDir * wallCheckDistance, 3f);
        if (enemyCheck != null) {
            Handles.color = Color.red;
            Handles.DrawWireDisc(enemyCheck.position, Vector3.forward, enemyCheckRadius);
        }
#endif
    }
}
