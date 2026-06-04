using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Enemy_Plant : Enemy
{
    [Header("Plant details")]
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
    }

    private void Attack() {
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

    protected override void HandleAnimator() {
        ; //we don't use xVelocity
    }
}
