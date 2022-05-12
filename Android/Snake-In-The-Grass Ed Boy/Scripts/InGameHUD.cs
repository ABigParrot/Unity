using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class InGameHUD : MonoBehaviour
{
    private static InGameHUD _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        
        transform.Find("PauseButton").GetComponent<Button_UI>().ClickFunc = () => GameHandler.PauseGame();
        transform.Find("PauseButton").GetComponent<Button_UI>().AddButtonSounds();
        
        Hide();
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public static void ShowStatic()
    {
        if (_instance != null)
        {
            Debug.Log("PauseWindow instance in ShowStatic = " + _instance);
            _instance.Show();
        }
    }
    public static void HideStatic()
    {
        _instance.Hide();
    }
}
