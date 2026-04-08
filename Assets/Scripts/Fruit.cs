using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitType
{
    Apple,
    Banana,
    Cherry
}

public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject pickupVfx;
    private GameManager gameManager;
    private Animator anim;

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start() {
        gameManager = GameManager.instance;
        SetRandomLookIfNeeded();
    }

    private void SetRandomLookIfNeeded() {
        if (!gameManager.FruitsHaveRandomLook()) {
            UpdateFruitVisual(); //enable default look
            return;
        }
        int randomIndex = Random.Range(0, 3);//generate a random number between 0,1,2
        anim.SetFloat("fruitIndex", randomIndex);
    }

    private void UpdateFruitVisual() {
        anim.SetFloat("fruitIndex", (int)fruitType);// evenif it is float
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        player player = collision.gameObject.GetComponent<player>();
        if (player != null) {
            gameManager.AddFruit();
            Destroy(gameObject);

            GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
        }
    }
}