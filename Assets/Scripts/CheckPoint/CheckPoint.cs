using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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