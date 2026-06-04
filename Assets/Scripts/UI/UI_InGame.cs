using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    public static UI_InGame instance;
    public UI_FadeEffect fadeEffect { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;
    [SerializeField] private GameObject pauseUI;

    private bool isPaused;

    private void Awake() {
        // 如果已经有实例了，销毁多余的自己
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }

    private void Start() {
        fadeEffect.ScreenFade(0, 1.5f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseButton();
        }
    }

    public void GoToMainMenu() {
        fadeEffect.ScreenFade(1, 1.5f, () => SceneManager.LoadScene("MainMenu"));
    }

    public void PauseButton() {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pauseUI.SetActive(isPaused);
    }

    public void UpdateFruitUI(int fruitsCollected, int fruitsTotal) {
        fruitText.text = fruitsCollected + "/" + fruitsTotal;
    }

    public void UpdateTimerUI(float timer) {
        timerText.text = timer.ToString("00") + "s";
    }
}
