using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    [Header("Pause Menu Parameters")]
    public static readonly float runningTimeScale = 1.5f;
    public GameObject PauseScreen;
    public AudioClip pauseSFX; // Set in Inspector
    public bool isPaused;

    //Panels
    public BuilderController uiBuilder;
    public GameObject uiScore;
    public GameObject uiBanner;
    public GameObject uiTimer;

    private TextMeshProUGUI uiScoreText;
    private TextMeshProUGUI uiBannerText;
    private TextMeshProUGUI uiTimerText;

    public void Awake() {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        PauseScreen.SetActive(false);
        isPaused = false;

        uiScoreText = uiScore.GetComponentInChildren<TextMeshProUGUI>();
        uiBannerText = uiBanner.GetComponentInChildren<TextMeshProUGUI>();
        uiTimerText = uiTimer.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() {
        if (Input.GetButtonDown ("Cancel")) {
            TogglePause();
        }
    }

    public void TogglePause() {
        if (isPaused)
            ClosePauseMenu();
        else
            OpenPauseMenu();
    }

    public void OpenPauseMenu() {
        isPaused = true; // pause
        Time.timeScale = 0;
        AudioManager.instance.audioSource.PlayOneShot(pauseSFX);
        PauseScreen.SetActive(true);
    }

    public void ClosePauseMenu() {
        isPaused = false; // unpause
        Time.timeScale = runningTimeScale;
        AudioManager.instance.audioSource.PlayOneShot(pauseSFX);
        PauseScreen.SetActive(false);
    }

    public void EnableScoreUI(bool b) {
        uiScore.gameObject.SetActive(b);
    }

    public void EnableBannerUI(bool b) {
        uiBanner.gameObject.SetActive(b);
    }

    public void EnableTimerUI(bool b) {
        uiTimer.gameObject.SetActive(b);
    }

    public void ScoreUIText(string s) {
        uiScoreText.text = s;
    }

    public void BannerUIText(string s) {
        uiBannerText.text = s;
    }

    public void TimerUIText(string s) {
        uiTimerText.text = s;
    }
}