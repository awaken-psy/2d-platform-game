using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效自毁辅助：挂载在 VFX 预制体上，由 Animator 的 Animation Event 调用 DestroyVFX，在动画结束时销毁对象。
/// </summary>
public class DestroyMeEvent : MonoBehaviour
{
    public void DestroyVFX()
    {
        Destroy(gameObject);
    }
}