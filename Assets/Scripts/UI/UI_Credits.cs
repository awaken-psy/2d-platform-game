using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Credits : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    [SerializeField] private RectTransform rectT;
    [SerializeField] private float scrollspeed;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float offScreenPosition = 1500f;

    private bool creditsSkipped;


    private void Awake() {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }

    private void Start() {
        fadeEffect.ScreenFade(0, 1.5f);
    }

    private void Update() {
        rectT.anchoredPosition += Vector2.up * scrollspeed * Time.deltaTime;

        if (rectT.anchoredPosition.y > offScreenPosition) {
            GoToMainMenu();
        }
    }

    public void SkipCredits() {
        if (!creditsSkipped) {
            scrollspeed *= 10;
            creditsSkipped = true;
        }
        else {
            GoToMainMenu();
        }
    }

    private void GoToMainMenuScene() {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void GoToMainMenu() {
        fadeEffect.ScreenFade(1, 1.5f, GoToMainMenuScene);
    }
}
