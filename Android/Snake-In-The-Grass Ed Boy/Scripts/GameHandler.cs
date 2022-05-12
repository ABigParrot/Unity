/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{
    private static GameHandler _instance;

    [SerializeField] private SnakeMobile snake;

    private LevelGrid levelGrid;
    
    private void Awake()
    {
        Time.timeScale = 1f;
        _instance = this;
        Score.InitializeStatic();
        //Debug.Log("GameHandler instance = " + _instance);

        Score.TrySetNewHighScore(200);
    }

    private void Start() {
        //SoundManager.PlaySound(SoundManager.Sound.BackgroundMusic);
        InGameHUD.ShowStatic();
        levelGrid = new LevelGrid(20, 20);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);

        
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused())
            {
                ResumeGame();
                SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
            }
            else
            {
                PauseGame();
                SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
            }
        }
    }

    public static void SnakeDied()
    {
        bool isNewHighScore = Score.TrySetNewHighScore();
        GameOverWindow.ShowStatic(isNewHighScore);
        ScoreWindow.HideStatic();
    }
    public static void ResumeGame()
    {
        
        PauseWindow.HideStatic();
        InGameHUD.ShowStatic();
        Time.timeScale = 1f;
    }
    public static void PauseGame()
    {
        PauseWindow.ShowStatic();
        InGameHUD.HideStatic();
        Time.timeScale = 0f;
    }

    public static bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }
}
