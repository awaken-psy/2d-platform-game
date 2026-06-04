using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    public string firstLevelName;

    [SerializeField] private GameObject[] UIElements;
    [SerializeField] private GameObject continueButton;

    private void Awake() {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }

    private void Start() {
        if (hadLevelProgress()) {
            continueButton.SetActive(true);
        }
        fadeEffect.ScreenFade(0, 1.5f);
    }

    public void SwitchToUI(GameObject UItoEnable) {
        foreach (GameObject UI in UIElements) {
            UI.SetActive(false);
        }
        UItoEnable.SetActive(true);
    }

    public void newGame() {
        fadeEffect.ScreenFade(1, 1.5f, loadLevelScene);
    }

    private void loadLevelScene() {
        SceneManager.LoadScene(firstLevelName);
    }

    private bool hadLevelProgress() => PlayerPrefs.GetInt("ContinueLevelNumber", 0) > 0;

    public void continueGame() {
        int continueLevelNumber = PlayerPrefs.GetInt("ContinueLevelNumber", 0);

        DifficultyManager.instance.LoadDifficulty();
        SceneManager.LoadScene("Level_" + continueLevelNumber);
    }
}
