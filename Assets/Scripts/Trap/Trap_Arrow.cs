using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Arrow : Trap_Trampoline
{
    [Header("Arrow Settings")]
    [SerializeField] private float rotationSpeed = 120f;

    [SerializeField] private float cooldown = 0f;
    [SerializeField] private bool rotateClockwise = true;

    [Space]
    [SerializeField] private float scaleUpSpeed = 2f;

    [SerializeField] private Vector3 targetScale;

    private void Start() {
        transform.localScale = new Vector3(.3f, .3f, .3f);
    }

    private void Update() {
        HandleScaleUp();
        HandleRotation();
    }

    private void HandleScaleUp() {
        if (transform.localScale.x < targetScale.x) {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation() {
        if (rotateClockwise) {
            transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        else {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    private void DestroyMe() {
        GameObject arrowPrefab = GameManager.instance.ArrowPrefab;
        GameManager.instance.CreateObject(arrowPrefab, transform, cooldown);
        Destroy(gameObject);
    }
}