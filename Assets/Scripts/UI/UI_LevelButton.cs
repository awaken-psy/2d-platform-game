using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI besttimeText;
    [SerializeField] private TextMeshProUGUI fruitsText;

    private int levelIndex;
    private string sceneName;

    public void SetupButton(int index) {
        levelIndex = index;
        sceneName = "Level_" + index;

        levelText.text = "Level_" + index;
        besttimeText.text = TimerInfoText();
        fruitsText.text = FruitsInfoText();
    }

    public void LoadLevel() {
        int difficultyIndex = (int)DifficultyManager.instance.difficulty;
        PlayerPrefs.SetInt("GameDifficulty", difficultyIndex);
        SceneManager.LoadScene(sceneName);
    }

    private string TimerInfoText() {
        int bestTime = PlayerPrefs.GetInt("Level" + levelIndex + "BestTime", 0);
        return "Best Time:" + (bestTime == 0 ? "No" : (bestTime.ToString("00") + "s"));
    }

    private string FruitsInfoText() {
        int fruitsCollected = PlayerPrefs.GetInt("Level" + levelIndex + "FruitsCollected", 0);
        int totalFruits = PlayerPrefs.GetInt("Level" + levelIndex + "TotalFruits", 0);
        return "Fruits:" + fruitsCollected.ToString() + "/" + (totalFruits == 0 ? "?" : totalFruits.ToString());
    }
}
