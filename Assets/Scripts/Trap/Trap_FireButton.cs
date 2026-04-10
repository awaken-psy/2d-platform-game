using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_FireButton : MonoBehaviour
{
    private Animator anim;
    private Trap_Fire trapFire;

    private void Awake() {
        anim = GetComponent<Animator>();
        trapFire = GetComponentInParent<Trap_Fire>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null) {
            anim.SetTrigger("activated");
            trapFire.SwitchOffFire();
        }
    }
}
