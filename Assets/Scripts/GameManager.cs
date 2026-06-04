using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局单例管理器，负责玩家重生、水果统计、延迟生成对象等核心流程。
/// 场景开始时自动统计所有水果数量，检查点通过 UpdateRespawnPosition 更新重生位置。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private UI_InGame inGameUI;

    [Header("Level Management")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int currentLevelIndex;
    private int nextLevelIndex;

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
        // 单例初始化：保留第一个实例，后续重复实例直接销毁
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        inGameUI = UI_InGame.instance;
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        nextLevelIndex = currentLevelIndex + 1;
        CollectFruitsInfo();
    }

    private void Update() {
        levelTimer += Time.deltaTime;
        inGameUI.UpdateTimerUI(levelTimer);
    }



    #region Respawn
    public void UpdateRespawnPosition(Transform newposition) => respawnPoint = newposition;

    public void respawnPlayer() {
        DifficultyManager dm = DifficultyManager.instance;
        if (dm != null && dm.difficulty == Difficulty.Hard)
            return;
        StartCoroutine(RespawnCoroutine());
    }

    /// <summary>
    /// 延迟重生协程：等待 respawnDelay 后在当前检查点位置生成新玩家。
    /// </summary>
    private IEnumerator RespawnCoroutine() {
        yield return new WaitForSeconds(respawnDelay);
        player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<player>();
    }
    #endregion

    #region fruits
    public int FruitCollected() => fruitsCollected;

    public void AddFruit() {
        fruitsCollected++;
        inGameUI.UpdateFruitUI(fruitsCollected, fruitsTotal);
    }

    public void RemoveFruit() {
        fruitsCollected--;
        inGameUI.UpdateFruitUI(fruitsCollected, fruitsTotal);
    }

    public bool FruitsHaveRandomLook() => fruitsHaveRandomLook;

    public void CreateObject(GameObject prefab, Transform target, float delay = 0) {
        StartCoroutine(CreateObjectCoroutine(prefab, target, delay));
    }

    /// <summary>
    /// 通用延迟生成协程：记录目标位置后等待 delay 秒再实例化 prefab。
    /// 用于 Trap_Arrow 等需要延迟刷新的陷阱。
    /// </summary>
    private IEnumerator CreateObjectCoroutine(GameObject prefab, Transform target, float delay) {
        Vector3 newposition = target.position;
        yield return new WaitForSeconds(delay);
        GameObject newObject = Instantiate(prefab, newposition, Quaternion.identity);
    }

    private void CollectFruitsInfo() {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        fruitsTotal = allFruits.Length;
        inGameUI.UpdateFruitUI(fruitsCollected, fruitsTotal);
        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", fruitsTotal);
    }
    #endregion

    #region Finish Level
    public void RestartLevel() {
        UI_InGame.instance.fadeEffect.ScreenFade(1, 1.5f, LoadCurrentLevelScene);
    }

    public void LevelFinished() {
        SaveLevelProgression();
        LoadNextScene();
        SaveFruitsInfo();
        SaveBestTime();
    }

    private void SaveFruitsInfo() {
        int fruitsCollectedBefore = PlayerPrefs.GetInt("Level" + currentLevelIndex + "FruitsCollected", 0);
        if (fruitsCollectedBefore < fruitsCollected)
            PlayerPrefs.SetInt("Level" + currentLevelIndex + "FruitsCollected", fruitsCollected);

        int totalFruitsInBank = PlayerPrefs.GetInt("TotalFruitsAmount");
        PlayerPrefs.SetInt("TotalFruitsAmount", totalFruitsInBank + fruitsCollected);
    }
    
    private void SaveBestTime() {
        int bestTime = PlayerPrefs.GetInt("Level" + currentLevelIndex + "BestTime", 0);
        if (bestTime == 0 || levelTimer < bestTime)
            PlayerPrefs.SetInt("Level" + currentLevelIndex + "BestTime", (int)levelTimer);
    }

    private void SaveLevelProgression() {
        if (!noMoreLevels()) {
            PlayerPrefs.SetInt("Level" + nextLevelIndex + "Unlocked", 1);
            PlayerPrefs.SetInt("ContinueLevelNumber", nextLevelIndex);
        }
    }

    private void LoadNextScene() {
        UI_FadeEffect fadeEffect = inGameUI.fadeEffect;

        if (noMoreLevels()) {
            fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
        }
        else {
            fadeEffect.ScreenFade(1, 1.5f, LoadNextLevelScene);
        }
    }

    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");
    private void LoadNextLevelScene() => SceneManager.LoadScene("Level_" + nextLevelIndex);
    private void LoadCurrentLevelScene() => SceneManager.LoadScene("Level_" + currentLevelIndex);

    private bool noMoreLevels() => SceneManager.sceneCountInBuildSettings - 2 == currentLevelIndex;

    #endregion
}