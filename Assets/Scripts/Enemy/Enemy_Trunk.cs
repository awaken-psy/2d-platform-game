using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Trunk : Enemy
{
    [Header("Trunk details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 7f;
    private float lastAttackTime;
    private float attackCooldown = 1.5f;

    protected override void Update() {
        base.Update();

        bool canAttack = Time.time > lastAttackTime + attackCooldown;

        if (isPlayerDetected && canAttack) {
            Attack();
        }

        if (isDead)
            return;

        HandleMovement();

        if (isGroundDetected)
            HandleTurnAround();
    }


    private void Attack() {
        idleTimer = idleDuration + attackCooldown;
        lastAttackTime = Time.time;
        anim.SetTrigger("attack");
    }

    private void createBullet() {
        Enemy_Bullet newbullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        newbullet.SetVelocity(new Vector2(facingDir * bulletSpeed, 0));

        if (facingDir == 1)
            newbullet.FlipSprite();

        Destroy(newbullet.gameObject, 10f);
    }

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
