using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    [SerializeField] private string PlayerLayerName = "Player";
    [SerializeField] private string GroundLayerName = "Ground";
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetVelocity(Vector2 velocity) => rb.velocity = velocity;

    public void FlipSprite() => sr.flipX = !sr.flipX;   
    
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PlayerLayerName)) {
            collision.gameObject.GetComponent<player>().Knockback(transform.position.x);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GroundLayerName)) {
            Destroy(gameObject);
        }
    }
}