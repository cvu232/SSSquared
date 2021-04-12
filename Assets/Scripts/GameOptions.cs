using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Characters {

    Robin,
    Olimar,
    Rob,
    Budbud

}

public class GameOptions : MonoBehaviour {

    public static GameOptions instance;

    public Color[] playerColours;

    [Header("UI Elements")]
    public Slider uiPlayerCount;
    public Slider uiLevelCount;
    public Toggle uiDeathScorePenalty;
    public Slider uiSFX;
    public Slider uiMusic;

    [Header ("Defaults")]
    [Range(2, 4)]
    public int defaultPlayerCount = 2;
    [Range(1, 8)]
    public int defaultLevelCount = 4;
    public bool defaultDeathScorePenalty;
    [Range(0, 10)]
    public int defaultSFX = 10;
    [Range(0, 10)]
    public int defaultMusic = 10;

    [Header ("Current Values")]
    [Range(2, 4)]
    public int playerCount;
    [Range(1, 8)]
    public int levelCount;
    public bool deathScorePenalty;
    [Range(0, 10)]
    public int SFX;
    [Range(0, 10)]
    public int music;

    [HideInInspector]
    public List<Characters> charactersPerPlayer = new List<Characters>();

    public GameOptions Awake() {
        if (instance && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }

        //Set current values to defaults on start
        playerCount = defaultPlayerCount;
        levelCount = defaultLevelCount;
        deathScorePenalty = defaultDeathScorePenalty;
        SFX = defaultSFX;
        music = defaultMusic;

        //Whenevever the scene changes, try to find UI elements again, in case player goes back to main menu
        SceneManager.sceneLoaded += FindUIElements;
        FindUIElements();

        return this;

    }

    private void Update() {
        if (uiPlayerCount)
            playerCount = Mathf.RoundToInt(uiPlayerCount.value);
        if (uiLevelCount)
            levelCount = Mathf.RoundToInt(uiLevelCount.value);
        if (uiDeathScorePenalty)
            deathScorePenalty = uiDeathScorePenalty.isOn;
        if (uiSFX)
            SFX = Mathf.RoundToInt(uiSFX.value);
        if (uiMusic)
            music = Mathf.RoundToInt(uiMusic.value);

        if (charactersPerPlayer.Count != playerCount) {
            charactersPerPlayer = new List<Characters>();
            for (int i = 0; i < playerCount; i++)
                charactersPerPlayer.Add((Characters)i);
        }

    }

    private void FindUIElements(Scene scene, LoadSceneMode mode) {
        FindUIElements();
    }

    private void FindUIElements() {

        try {
            uiPlayerCount = GameObject.Find("UISlider.PlayerCount").GetComponent<Slider>();
            uiPlayerCount.value = playerCount;
        } catch { Debug.LogWarning("Coult not find Level Count slider"); }
        try {
            uiLevelCount = GameObject.Find("UISlider.LevelCount").GetComponent<Slider>();
            uiLevelCount.value = levelCount;
        } catch { Debug.LogWarning("Coult not find Level Count slider"); }
        try {
            uiDeathScorePenalty = GameObject.Find("UIToggle.DeathScorePenalty").GetComponent<Toggle>();
            uiDeathScorePenalty.isOn = deathScorePenalty;
        } catch { Debug.LogWarning("Coult not find Death Score Penalty toggle");  }
        try {
            uiSFX = GameObject.Find("UISlider.SFX").GetComponent<Slider>();
            uiSFX.value = SFX;
        } catch { Debug.LogWarning("Coult not find SFX slider");  }
        try {
            uiMusic = GameObject.Find("UISlider.Music").GetComponent<Slider>();
            uiMusic.value = music;
        } catch { Debug.LogWarning("Coult not find Music slider");  }

    }

}