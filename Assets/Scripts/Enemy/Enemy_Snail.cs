using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Snail : Enemy
{
    [Header("Snail details")]
    [SerializeField] private Enemy_Snailbody bodyPrefab;
    [SerializeField] private float maxspeed;
    private bool hasBody = true;

    protected override void Update() {
        base.Update();
        if (isDead)
            return;
        HandleMovement();
        if (isGroundDetected)
            HandleTurnAround();
    }

    public override void Die() {
        if (hasBody) {
            canMove = false;
            hasBody = false;
            rb.velocity = Vector2.zero;
            anim.SetTrigger("hit");
            idleDuration = 0;
        }
        else if (!hasBody && !canMove) {
            anim.SetTrigger("hit");
            canMove = true;
            moveSpeed = maxspeed;
        }
        else {
            base.Die();
        }
    }
    private void HandleTurnAround() {
        if (!isGroundInfrontDetected && hasBody || isWallDetected) {

            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement() {
        if (idleTimer > 0)
            return;

        if (canMove == false)
            return;

        rb.velocity = new Vector2(facingDir * moveSpeed, rb.velocity.y);
    }

    private void CreateBody() {
        Enemy_Snailbody body = Instantiate(bodyPrefab, transform.position, Quaternion.identity);

        if (Random.Range(0, 100) < 50) {
            deathRotationDir *= -1;
        }

        body.SetupBody(deathImpact, deathRotationDir * deathRotationSpeed, facingDir);

        Destroy(body.gameObject, 10f);
    }

    protected override void Flip() {
        base.Flip();

        if (!hasBody) {
            anim.SetTrigger("wallhit");
        }
    }
}
