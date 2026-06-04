using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 火焰陷阱：默认持续激活，玩家踩上关联的 Trap_FireButton 后进入关闭协程，
/// 经过 offDuration 秒后自动重新激活。
/// </summary>
public class Trap_Fire : MonoBehaviour
{
    [SerializeField] private float offDuration;
    [SerializeField] private Trap_FireButton fireButton;

    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;

    void Awake() {
        anim = GetComponent<Animator>();
        fireCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start() {
        if (fireButton == null)
            Debug.LogWarning("You don't have fire button on " + gameObject.name + "!");
        setFire(true);
    }

    /// <summary>
    /// 外部调用接口（由 Trap_FireButton 触发）：关闭火焰并在 offDuration 后重新激活。
    /// </summary>
    public void SwitchOffFire() {
        if (!isActive)
            return;
        StartCoroutine(FireCourtine());
    }

    private IEnumerator FireCourtine() {
        setFire(false);
        yield return new WaitForSeconds(offDuration);
        setFire(true);
    }

    private void setFire(bool active) {
        anim.SetBool("active", active);
        fireCollider.enabled = active;
        isActive = active;
    }
}
