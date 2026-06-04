using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 伤害触发器：挂载在敌人或陷阱子对象上，带有 Trigger Collider。
/// 玩家进入时根据其相对于触发器的 X 坐标决定击退方向。
/// </summary>
public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null) {
            player.Damage();
            player.Knockback(transform.position.x);
        }
    }
}