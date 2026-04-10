using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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