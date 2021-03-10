using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Pause Menu Parameters")]
    public float runningTimeScale;
    public GameObject PauseScreen;
    public bool isPaused;


    //Panels
    public BuilderController uiBuildBar;
    public GameObject uiScore;
    public GameObject uiBanner;
    public GameObject uiTimer;

    private TextMeshProUGUI uiScoreText;
    private TextMeshProUGUI uiBannerText;
    private TextMeshProUGUI uiTimerText;

    void Awake()
    {
        instance = this;

        runningTimeScale = Time.timeScale;
        PauseScreen.SetActive(false);
        isPaused = false;

        uiScoreText = uiScore.GetComponentInChildren<TextMeshProUGUI>();
        uiBannerText = uiBanner.GetComponentInChildren<TextMeshProUGUI>();
        uiTimerText = uiTimer.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            OpenPauseMenu();
        }
        else
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            ClosePauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        isPaused = true; // pause
        Time.timeScale = 0;
        PauseScreen.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        isPaused = false; // unpause
        Time.timeScale = runningTimeScale;
        PauseScreen.SetActive(false);
    }

    public void EnableScoreUI(bool b)
    {
        uiScore.gameObject.SetActive(b);
    }

    public void EnableBannerUI(bool b)
    {
        uiBanner.gameObject.SetActive(b);
    }

    public void EnableTimerUI(bool b)
    {
        uiTimer.gameObject.SetActive(b);
    }

    public void ScoreUIText(string s)
    {
        uiScoreText.text = s;
    }

    public void BannerUIText(string s)
    {
        uiBannerText.text = s;
    }

    public void TimerUIText(string s)
    {
        uiTimerText.text = s;
    }
}
