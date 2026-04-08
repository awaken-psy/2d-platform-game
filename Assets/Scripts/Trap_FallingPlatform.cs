using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D[] colliders;

    [SerializeField] private float speed = .75f;
    [SerializeField] private float travelDistance;
    public Vector3[] wayPoints;
    private int wayPointIndex;
    private bool canMove = false;

    [Header("Platform Fall Detail")]
    [SerializeField] private float impactSpeed = 3f;

    [SerializeField] private float impactDuration = 0.1f;
    private float impactTimer;
    private bool impactHappened;

    [Space]
    [SerializeField] private float fallDelay = 0.5f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private IEnumerator Start() {
        SetUpWayPoints();
        float randomDelay = Random.Range(0f, 1f);
        yield return new WaitForSeconds(randomDelay);
        canMove = true;
    }

    private void Update() {
        HandleImpact();
        HandleMovement();
    }

    private void SetUpWayPoints() {
        wayPoints = new Vector3[2];
        float yOffset = travelDistance / 2;
        wayPoints[0] = transform.position + Vector3.down * yOffset;
        wayPoints[1] = transform.position + Vector3.up * yOffset;
    }

    private void HandleMovement() {
        if (!canMove) {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[wayPointIndex], speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < 0.1f) {
            wayPointIndex++;
            if (wayPointIndex >= wayPoints.Length) {
                wayPointIndex = 0;
            }
        }
    }

    private void HandleImpact() {
        if (impactTimer < 0) return;
        impactTimer -= Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, impactSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (impactHappened)
            return;

        player player = collision.gameObject.GetComponent<player>();
        if (player != null) {
            Invoke(nameof(SwitchOffPlatform), fallDelay);
            impactTimer = impactDuration;
            impactHappened = true;
        }
    }

    private void SwitchOffPlatform() {
        canMove = false;

        anim.SetTrigger("deactivated");
        rb.isKinematic = false;
        rb.gravityScale = 4f;
        rb.drag = .5f;
        foreach (Collider2D col in colliders) {
            col.enabled = false;
        }
    }
}