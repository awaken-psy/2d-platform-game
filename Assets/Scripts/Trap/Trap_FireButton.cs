using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 火焰开关按钮：玩家进入触发器后通知父对象的 Trap_Fire 关闭火焰，并播放自身激活动画。
/// </summary>
public class Trap_FireButton : MonoBehaviour
{
    private Animator anim;
    private Trap_Fire trapFire;

    private void Awake() {
        anim = GetComponent<Animator>();
        trapFire = GetComponentInParent<Trap_Fire>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null) {
            anim.SetTrigger("activated");
            trapFire.SwitchOffFire();
        }
    }
}
