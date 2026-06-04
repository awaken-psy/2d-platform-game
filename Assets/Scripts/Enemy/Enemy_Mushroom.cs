using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 蘑菇敌人：基础巡逻 AI，沿当前方向直线移动，遇到墙壁或前方无地面时转身并进入短暂 idle。
/// </summary>
public class Enemy_Mushroom : Enemy
{
    protected override void Update() {
        base.Update();
        if (isDead)
            return;
        HandleMovement();
        if (isGroundDetected)
            HandleTurnAround();
    }

    /// <summary>
    /// 边缘/墙壁检测：前方无地面或碰到墙时转身，并停顿 idleDuration 秒。
    /// </summary>
    private void HandleTurnAround() {
        if (!isGroundInfrontDetected || isWallDetected) {

            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement() {
        if (idleTimer > 0)
            return;

        rb.velocity = new Vector2(facingDir * moveSpeed, rb.velocity.y);
    }

}
