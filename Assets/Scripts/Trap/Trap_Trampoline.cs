using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 蹦床陷阱：玩家进入触发器时沿自身 up 方向施加瞬时推力，并播放激活动画。
/// Trap_Arrow 继承此类以复用触发逻辑。
/// </summary>
public class Trap_Trampoline : MonoBehaviour
{
    protected Animator anim;
    [SerializeField] private float pushPower = 25f;
    [SerializeField] private float pushDuration = .5f;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null) {
            player.Push(transform.up * pushPower, pushDuration);
            anim.SetTrigger("activated");
        }
    }
}