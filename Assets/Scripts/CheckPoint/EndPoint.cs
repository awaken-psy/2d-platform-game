using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 终点触发器：玩家到达时播放通关动画，并输出日志（后续可接入关卡切换逻辑）。
/// </summary>
public class EndPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null)
        {
            anim.SetBool("active", true);
            GameManager.instance.LevelFinished();
        }
    }
}