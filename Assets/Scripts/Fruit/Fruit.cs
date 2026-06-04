using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitType
{
    Apple,
    Banana,
    Cherry
}

/// <summary>
/// 可收集水果：支持按 FruitType 显示固定外观，或在 GameManager 设置随机外观时随机选择一种。
/// 被玩家拾取后增加计数、生成特效并销毁自身。
/// </summary>
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
            UpdateFruitVisual(); // 使用 Inspector 中指定的默认外观
            return;
        }
        int randomIndex = Random.Range(0, 3); // 生成 0~2 的随机索引
        anim.SetFloat("fruitIndex", randomIndex);
    }

    private void UpdateFruitVisual() {
        anim.SetFloat("fruitIndex", (int)fruitType); // Animator 使用 float 参数存储索引
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