using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Fire : MonoBehaviour
{
    [SerializeField] private float offDuration;
    [SerializeField] private Trap_FireButton fireButton;

    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;

    void Awake() {
        anim = GetComponent<Animator>();
        fireCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start() {
        if (fireButton == null)
            Debug.LogWarning("You don't have fire button on " + gameObject.name + "!");
        setFire(true);
    }

    public void SwitchOffFire() {
        if (!isActive)
            return;
        StartCoroutine(FireCourtine());
    }

    private IEnumerator FireCourtine() {
        setFire(false);
        yield return new WaitForSeconds(offDuration);
        setFire(true);
    }

    private void setFire(bool active) {
        anim.SetBool("active", active);
        fireCollider.enabled = active;
        isActive = active;
    }
}
