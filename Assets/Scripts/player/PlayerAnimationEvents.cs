using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家动画事件中转器：挂载在玩家子对象（Animator 所在物体）上，
/// 用于将 Animator 的 Animation Event 回调转发到父对象的 player 脚本。
/// </summary>
public class PlayerAnimationEvents : MonoBehaviour
{
    private player player;

    private void Awake()
    {
        player = GetComponentInParent<player>();
    }

    /// <summary>
    /// 由重生入场动画的最后一帧调用，通知玩家恢复控制与物理。
    /// </summary>
    public void FinishRespawn() => player.RespawnFinished(true);
}