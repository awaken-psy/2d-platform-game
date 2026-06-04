using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 鸡敌人：带有检测范围的追击型 AI。当玩家进入面向方向的 detectionRange 内时进入 aggro 状态并追逐，
/// 失去检测后仍会持续追逐 aggroDuration 秒，随后停止。
/// </summary>
public class Enemy_Chicken : Enemy
{
    [Header("Chicken details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;

    private float aggroTimer;
    private bool canFlip;

    protected override void Update() {
        base.Update();
        aggroTimer -= Time.deltaTime;

        if (isDead)
            return;
     
        // 玩家被检测到则重置 aggro 计时器并允许移动
        if (isPlayerDetected) {
            canMove = true;
            aggroTimer = aggroDuration;
        }
        // aggro 时间耗尽后停止移动
        if (aggroTimer < 0)
            canMove = false;

        HandleMovement();
        if (isGroundDetected)
            HandleTurnAround();
    }

    private void HandleTurnAround() {
        if (!isGroundInfrontDetected || isWallDetected) {
            Flip();
            canMove = false;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement() {
        if (!canMove)
            return;

        HandleFlip(player.transform.position.x);

        rb.velocity = new Vector2(facingDir * moveSpeed, rb.velocity.y);
    }

    /// <summary>
    /// 重写翻转逻辑：加入 0.3 秒延迟，避免在边缘快速来回翻转导致抖动。
    /// </summary>
    protected override void HandleFlip(float xValue) {
        if (facingRight && xValue < transform.position.x || !facingRight && xValue > transform.position.x) {
            if (canFlip) {
                canFlip = false;
                Invoke(nameof(Flip), .3f);
            }
        }
    }

    override protected void Flip() {
        base.Flip();
        canFlip = true;
    }

}
