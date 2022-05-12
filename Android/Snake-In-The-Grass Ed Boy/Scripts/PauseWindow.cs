using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class PauseWindow : MonoBehaviour
{
    private static PauseWindow _instance;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        
        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        transform.Find("resumeBtn").GetComponent<Button_UI>().ClickFunc = () => GameHandler.ResumeGame();
        transform.Find("resumeBtn").GetComponent<Button_UI>().AddButtonSounds();
        
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().ClickFunc = () => Loader.Load(Loader.Scene.MainMenu);
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().AddButtonSounds();
        
        Hide();
    }
    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            Debug.Log("PauseWindow instance = " + _instance);
        }
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
