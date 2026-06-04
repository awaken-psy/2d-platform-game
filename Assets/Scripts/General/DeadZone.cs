using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡区域：玩家进入后立刻死亡并触发 GameManager 的重生协程。
/// 通常放置在场景底部或危险区域。
/// </summary>
public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null) {
            player.Damage();
            player.Die();
            GameManager.instance.respawnPlayer();
        }

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.Die();
        }
    }

}