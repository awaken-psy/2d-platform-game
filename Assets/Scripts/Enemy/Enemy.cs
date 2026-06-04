using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 敌人基类，封装通用行为：移动、翻转、死亡旋转、地面/墙壁碰撞检测。
/// 子类通过覆写 Update 添加具体 AI（巡逻、追击等）。
/// </summary>
public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    protected Transform player;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D[] cd;


    [Header("General Details")]
    [SerializeField] protected float moveSpeed = 2f;
    protected bool canMove = true;
    [SerializeField] protected float idleDuration = .5f;
    protected float idleTimer;

    [Header("Death Details")]
    [SerializeField] protected float deathImpact = 2f;
    [SerializeField] protected float deathRotationSpeed = 150f;
    protected int deathRotationDir = -1;
    protected bool isDead;

    [Header("Basic Collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = 0.7f;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected Transform GroundCheck;
    [SerializeField] protected float playerDetectionDistance = 15f;
    protected bool isPlayerDetected;
    protected bool isGroundDetected;
    protected bool isGroundInfrontDetected;
    protected bool isWallDetected;

    protected int facingDir = -1;
    protected bool facingRight = false;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponentsInChildren<BoxCollider2D>();
    }

    protected virtual void Start() {
        // 每秒刷新一次玩家引用，避免重生后引用失效
        InvokeRepeating(nameof(UpdatePlayersRef), 0, 1);

        if (sr.flipX == true && !facingRight) {
            sr.flipX = false;
            Flip();
        }
    }

    protected virtual void Update() {
        HandleCollision();
        HandleAnimator();

        idleTimer -= Time.deltaTime;

        if (isDead) {
            HandleDeathRotation();
        }
    }

    private void UpdatePlayersRef() {
        if (player == null)
            player = GameManager.instance.player.transform;
    }

    #region anim
    protected virtual void HandleAnimator() {
        anim.SetFloat("xVelocity", rb.velocity.x);
    }
    #endregion

    #region death
    
    public virtual void Die() {
        foreach (var collider in cd) {
            collider.enabled = false;
        }

        anim.SetTrigger("hit");
        rb.velocity = new Vector2(rb.velocity.x, deathImpact);
        isDead = true;

        if (Random.Range(0, 100) < 50) {
            deathRotationDir *= -1;
        }

        Destroy(gameObject, 10f);
    }

    private void HandleDeathRotation() {
        transform.Rotate(0f, 0f, deathRotationSpeed * deathRotationDir * Time.deltaTime);
    }

    #endregion death

    #region flip

    protected virtual void HandleFlip(float xValue) {
        if (facingRight && xValue < transform.position.x || !facingRight && xValue > transform.position.x)
            Flip();
    }

    protected virtual void Flip() {
        facingDir *= -1;
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    [ContextMenu("Change Facing Direction")]
    public void FlipDefaultFacingDirection() {
        sr.flipX = !sr.flipX;
    }

    #endregion flip

    #region collision

    protected virtual void HandleCollision() {
        isGroundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInfrontDetected = Physics2D.Raycast(GroundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, playerDetectionDistance, whatIsPlayer);
    }

    protected virtual void OnDrawGizmos() {
#if UNITY_EDITOR
        Handles.color = Color.black;
        Handles.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance, 5f);
        Handles.DrawLine(GroundCheck.position, GroundCheck.position + Vector3.down * groundCheckDistance, 5f);
        Handles.DrawLine(transform.position, transform.position + Vector3.right * facingDir * wallCheckDistance, 5f);
        Handles.DrawLine(transform.position, transform.position + Vector3.right * facingDir * playerDetectionDistance, 5f);
#endif
    }

    #endregion collision

}
