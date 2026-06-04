using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonParent;

    [SerializeField] private bool[] levelsUnlocked;

    private void Start() {
        LoadLevelsInfo();
        CreateLevelButtons();
    }

    private void CreateLevelButtons() {
        int levelamount = SceneManager.sceneCountInBuildSettings - 1;

        for (int i = 1; i < levelamount; i++) {
            if (!IsLevelUnlocked(i)) {
                return;
            }

            UI_LevelButton button = Instantiate(buttonPrefab, buttonParent);
            button.SetupButton(i);
        }
    }

    private bool IsLevelUnlocked(int index) => levelsUnlocked[index];

    private void LoadLevelsInfo() {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        levelsUnlocked = new bool[levelsAmount];

        for (int i = 1; i < levelsAmount; i++) {
            levelsUnlocked[i] = PlayerPrefs.GetInt("Level" + i + "Unlocked", 0) == 1;
        }
        levelsUnlocked[1] = true;
    }
}
