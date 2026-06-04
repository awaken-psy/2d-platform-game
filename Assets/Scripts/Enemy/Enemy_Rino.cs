using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy_Rino : Enemy
{
    [Header("Rino details")]
    [SerializeField] private float maxSpeed;
    private float defaultSpeed;
    [SerializeField] private float speedUpRate;
    [SerializeField] private Vector2 impactPower;


    protected override void Start() {
        base.Start();

        canMove = false;
        defaultSpeed = moveSpeed;
    }
    protected override void Update() {
        base.Update();
        HandleCharge();
    }

    private void HandleCharge() {
        if (canMove == false)
            return;

        HandleSpeedUp();

        rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);

        if (isWallDetected) {
            WallHit();
        }

        if (!isGroundInfrontDetected) {
            TurnAround();
        }
    }

    private void SpeedReset() {
        moveSpeed = defaultSpeed;
    }

    private void HandleSpeedUp() {
        moveSpeed += Time.deltaTime * speedUpRate;
        if (moveSpeed >= maxSpeed) {
            moveSpeed = maxSpeed;
        }
    }

    private void TurnAround() {
        SpeedReset();
        canMove = false;
        rb.velocity = Vector2.zero;
        Flip();


    }

    private void WallHit() {
        canMove = false;
        SpeedReset();
        anim.SetBool("hitWall", true);
        rb.velocity = new Vector2(impactPower.x * -facingDir, impactPower.y);
    }

    private void ChargeIsOver() {
        canMove = false;
        anim.SetBool("hitWall", false);
        Invoke(nameof(Flip), 1);
    }

    protected override void HandleCollision() {
        base.HandleCollision();

        if (isPlayerDetected && isGroundDetected) {
            canMove = true;
        }
    }
}
