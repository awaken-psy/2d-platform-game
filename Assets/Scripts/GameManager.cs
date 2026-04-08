using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Management")]
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public player player;

    [Header("Fruit Management")]
    public bool fruitsHaveRandomLook;

    public int fruitsCollected;
    public int fruitsTotal;

    [Header("Checkpoints")]
    public bool canBeReactivate;

    [Header("Traps")]
    public GameObject ArrowPrefab;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        CollectFruitsInfo();
    }

    private void CollectFruitsInfo() {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        fruitsTotal = allFruits.Length;
    }

    public void UpdateRespawnPosition(Transform newposition) => respawnPoint = newposition;

    public void respawnPlayer() => StartCoroutine(RespawnCoroutine());

    private IEnumerator RespawnCoroutine() {
        yield return new WaitForSeconds(respawnDelay);
        player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<player>();
    }

    public void AddFruit() => fruitsCollected++;

    public bool FruitsHaveRandomLook() => fruitsHaveRandomLook;

    public void CreateObject(GameObject prefab, Transform target, float delay = 0) {
        StartCoroutine(CreateObjectCoroutine(prefab, target, delay));
    }

    private IEnumerator CreateObjectCoroutine(GameObject prefab, Transform target, float delay) {
        Vector3 newposition = target.position;
        yield return new WaitForSeconds(delay);
        GameObject newObject = Instantiate(prefab, newposition, Quaternion.identity);
    }
}