using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Difficulty : MonoBehaviour
{
    private DifficultyManager difficultyManager;

    private void Start() {
        difficultyManager = DifficultyManager.instance;
    }
    
    public void SetEasy() => difficultyManager.SetDifficulty(Difficulty.Easy);
    public void SetNormal() => difficultyManager.SetDifficulty(Difficulty.Normal);
    public void SetHard() => difficultyManager.SetDifficulty(Difficulty.Hard);
}
