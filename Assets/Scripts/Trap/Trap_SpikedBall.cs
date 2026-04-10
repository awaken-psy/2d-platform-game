using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_SpikedBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D spikerb;
    [SerializeField] private float pushForce;

    void Start() {
        Vector2 pushVector = new Vector2(pushForce, 0);
        spikerb.AddForce(pushVector, ForceMode2D.Impulse);
    }
}
