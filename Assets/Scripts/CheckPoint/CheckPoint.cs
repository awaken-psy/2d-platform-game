using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检查点逻辑：玩家进入触发器后更新 GameManager 的重生位置，并播放激活动画。
/// 若已激活且不允许重复激活，则忽略后续触发。
/// </summary>
public class CheckPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private bool active;
    [SerializeField] private bool canBeReactivate;

    private void Start()
    {
        canBeReactivate = GameManager.instance.canBeReactivate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && !canBeReactivate)
            return;
        player player = collision.GetComponent<player>();
        if (player != null)
            ActivateCheckpoint();
    }

    private void ActivateCheckpoint()
    {
        active = true;
        anim.SetBool("active", active);
        GameManager.instance.UpdateRespawnPosition(transform);
    }
}