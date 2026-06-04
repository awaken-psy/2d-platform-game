using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 起点触发器：玩家离开起点区域时激活动画（如旗帜升起），标志关卡正式开始。
/// </summary>
public class StartPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();

    private void OnTriggerExit2D(Collider2D collision)
    {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null)
        {
            anim.SetTrigger("activate");
        }
    }
}