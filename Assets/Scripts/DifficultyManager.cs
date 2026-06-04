using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy = 1,
    Normal,
    Hard
}
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    public Difficulty difficulty;

    private void Awake() {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetDifficulty(Difficulty dif) => difficulty = dif;
    public void LoadDifficulty() {
        int difficultyIndex = PlayerPrefs.GetInt("GameDifficulty", 1);
        difficulty = (Difficulty)difficultyIndex;
    }
}
