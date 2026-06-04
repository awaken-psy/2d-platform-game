using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 尖刺球陷阱：启动时沿 X 轴施加瞬时冲量，使其摆动或飞出。
/// </summary>
public class Trap_SpikedBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D spikerb;
    [SerializeField] private float pushForce;

    void Start() {
        Vector2 pushVector = new Vector2(pushForce, 0);
        spikerb.AddForce(pushVector, ForceMode2D.Impulse);
    }
}
