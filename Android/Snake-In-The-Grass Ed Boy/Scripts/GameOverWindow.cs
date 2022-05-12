using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    //Makes an instance of this script
    private static GameOverWindow instance;
    private void Awake()
    {
        instance = this;
        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.SampleScene);
        };
        Hide();
    }

    private void Show(bool isNewHighScore)
    {
        gameObject.SetActive(true);
        transform.Find("NewHighScoreText").gameObject.SetActive(isNewHighScore);
        
        transform.Find("ScoreText").GetComponent<Text>().text = Score.GetScore().ToString();
        transform.Find("HighScoreText").GetComponent<Text>().text = "HIGHSCORE " + Score.GetHighScore();
        
        InGameHUD.HideStatic();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public static void ShowStatic(bool isNewHighScore)
    {
        //Used by the GameHandler script
        instance.Show(isNewHighScore);
    }
}
