using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private player player;

    private void Awake()
    {
        player = GetComponentInParent<player>();
    }

    public void FinishRespawn() => player.RespawnFinished(true);
}