using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 旋转箭矢陷阱：生成后逐渐放大并持续旋转，命中玩家时继承 Trap_Trampoline 的弹起效果。
/// 被销毁后通过 GameManager 延迟生成新的箭矢，实现循环刷新。
/// </summary>
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

    /// <summary>
    /// 由动画事件或外部调用：销毁自身并请求 GameManager 在 cooldown 后生成新箭矢。
    /// </summary>
    private void DestroyMe() {
        GameObject arrowPrefab = GameManager.instance.ArrowPrefab;
        GameManager.instance.CreateObject(arrowPrefab, transform, cooldown);
        Destroy(gameObject);
    }
}