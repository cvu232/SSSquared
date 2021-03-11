using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Phases {
    buildPhase,
    racePhase
}

public class GamePhaseManager : MonoBehaviour {
    public static GamePhaseManager instance;

    public Phases currentGamePhase;// { get; private set; }

    [Header("Container Objects")]
    private GameObject levelsContainer; // object that holds all levels
    private GameObject playersContainer; // object that holds all players

    [Header("Build Phase Parameters")]
    public float buildPhaseDuration = 60; // initial build mode time in seconds
    [Tooltip("What key should be pressed to start the next player's build turn")]
    public string onReadyUpButtonName;
    public string onEndTurnButtonName;
    public int maxLevelCount = 4;
    public bool useFixedLevelsPerPlayer;
    public int fixedLevelsPerPlayer;
    public GameObject emptyLevelPrefab;
    public float emptyLevelSpacing;

    [Header("Race Phase Parameters")]
    public float racePhaseDuration = 150; // initial platform mode time in seconds
    public bool[] readyCheck; // ready check before starting game

    [Header("Players")]
    public Player player1;
    public Player player2;

    [Header("Misc. Parameters")]
    public float currentPhaseTimer; // initial platform mode time in seconds

    public bool blockEffectsEnabled; // are block effects active

    public List<Level> levels;
    public List<Player> players;

    public BlockBase[] blocks;

    public int currentLevel;
    public int currentBuilder;


    private void Awake() // Singleton
    {
        if (instance != null && instance != this)
            Destroy(this);
        else {

            instance = this;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        }

    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1) {
        Debug.LogWarning("Scene change detected!");
    }

    private void Start()
    {
        InitialSetup();
    }

    private void InitialSetup() {
        // Initialize some variables //
        Physics2D.queriesStartInColliders = true;

        blockEffectsEnabled = false;


        UIManager.instance.uiBuildBar.Initial();

        levelsContainer = GameObject.Find("LevelsCONTAINER");
        playersContainer = GameObject.Find("PlayersCONTAINER");

        Time.timeScale = UIManager.runningTimeScale;

        currentGamePhase = Phases.buildPhase;
        currentLevel = 0;
        currentBuilder = 0;

        //Placeholder players
        players = new List<Player>(FindObjectsOfType<Player>());
        DeactivatePlayers();

        // If players should have a fixed number of levels to build, override the maxLevelCount variable here
        if (useFixedLevelsPerPlayer)
            maxLevelCount = players.Count * fixedLevelsPerPlayer;
        else
            fixedLevelsPerPlayer = -1;

        //Generate levels and add them to 'levels' array
        levels = new List<Level>();
        for (int i = 0; i < maxLevelCount; i++) {
            Level newLevel = Instantiate(emptyLevelPrefab).GetComponent<Level>();
            newLevel.transform.parent = levelsContainer.transform;
            newLevel.transform.localPosition = Vector3.up * (i * emptyLevelSpacing);
            if (i % 2 == 0)
                newLevel.transform.localScale = new Vector3(-1, 1, 1);
            levels.Add(newLevel);
            Debug.Log("Generating Level #" + i + "...");
        }

        foreach (Level lvl in levels)
            Debug.Log("Level: " + lvl.name);

        StartCoroutine(BuildPhase());
    }

    private void Update() {
        currentPhaseTimer = Mathf.Max (currentPhaseTimer - Time.deltaTime, 0);
        if (UIManager.instance.uiTimer)
            UIManager.instance.TimerUIText(string.Format("Remaining Time : {0:F2}", currentPhaseTimer));

        //SkipBehaviour
        if (Input.GetButtonDown(onEndTurnButtonName) && currentGamePhase == Phases.buildPhase)
            currentPhaseTimer = 0;

        // Active Score UI output
        if (UIManager.instance.uiScore.activeSelf && currentGamePhase == Phases.racePhase)
            UIManager.instance.ScoreUIText(string.Format("Score: {0}:{1}", players[0].score, players[1].score));

    }

    IEnumerator BuildPhase() {
        //Setup for Build Phase
        currentGamePhase = Phases.buildPhase; // Set current phase
        UIManager.instance.EnableBannerUI(true); // Enable top canvas

        for (currentLevel = 0, currentBuilder = 0; currentLevel < levels.Count; currentLevel++, currentBuilder++) {
            //Initialize current player's build phase
            //Move Camera
            //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, currentLevel * emptyLevelSpacing, Camera.main.transform.position.z);

            //UI to indicate which button to start
            UIManager.instance.BannerUIText(
                    string.Format("Player {0}: press {1} to start building" , (currentBuilder + 1), onReadyUpButtonName));
            // Wait for Player to be ready to build
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName));
            //Show end turn instruction
            //Set UI to indicate current builder player
            UIManager.instance.BannerUIText(
                string.Format("Player {0} is building Level {1}", currentBuilder + 1, currentLevel + 1));
            UIManager.instance.uiBuildBar.Activate(); // Enable builder's canvas
            UIManager.instance.EnableTimerUI(true);
            currentPhaseTimer = buildPhaseDuration; // Start build timer

            //Wait for Build Phase
            yield return new WaitUntil(() => currentPhaseTimer <= 0.001f); // Wait until building finishes
            currentPhaseTimer = 0;
            //End current player's build phase
            UIManager.instance.uiBuildBar.Deactivate();

            //Increase the current player index, loop to start if index gets out of bounds
            if (currentBuilder >= players.Count)
                currentBuilder = 0;
            levels[currentBuilder].builderIndex = currentBuilder;

            //Acknowledge next player to build or build phase is complete
            if (currentLevel < levels.Count - 1)
                UIManager.instance.BannerUIText(
                    string.Format("Press {0} to begin building the next level", onReadyUpButtonName));
            else
                UIManager.instance.BannerUIText(
                    string.Format("Press {0} to proceed to the Racing Phase", onReadyUpButtonName));
        }

        //Start Next Phase
        StartCoroutine(RacePhase()); // Start race mode
    }

    IEnumerator RacePhase() {
        //Setup for Race Phase
        Physics2D.queriesStartInColliders = false;
        try {
            GameObject.Find("GridMesh").SetActive(false);
        } catch { }

        currentGamePhase = Phases.racePhase; // Set current phase
            UIManager.instance.EnableScoreUI(true);
        ActivateBlockEffects(); // Enable block effects
        //TODO: Generate array of ready status booleans

        for (currentLevel = 0; currentLevel < levels.Count; currentLevel++) {
            //Move camera and all players to current level's start area
            //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, currentLevel * emptyLevelSpacing, Camera.main.transform.position.z);
            MovePlayersTo(levels[currentLevel].spawnPoint);


            //Wait for all players to be ready
            UIManager.instance.BannerUIText(
                string.Format("Press {0} begin racing", onReadyUpButtonName));
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName)); // Wait until next player presses 'ready' button
            //Set UI to indicate current level and its builder
            UIManager.instance.BannerUIText(
                string.Format("Level: {0}. Built by Player {1} ", (currentLevel + 1), levels[currentLevel].builderIndex));
            ActivatePlayers();
            currentPhaseTimer = racePhaseDuration; // Set build timer

            //Wait for Level Timeout
            yield return new WaitUntil(() => currentPhaseTimer <= 0.001f); // Wait until race finishes
            currentPhaseTimer = 0;
            DeactivatePlayers();


            if (!levels[currentLevel].winner) {
                players[levels[currentLevel].builderIndex].score -= 3;
                UIManager.instance.BannerUIText(
                    string.Format("There was no winner for this level... Player {0} suffers a score penalty.", levels[currentLevel].builderIndex + 1));
            } else if (levels[currentLevel].winner) {
                levels[currentLevel].winner.score += 3;
                UIManager.instance.BannerUIText(
                    string.Format("Player {0} is the winner of this level!", players.IndexOf(levels[currentLevel].winner) + 1));

            }

            //End current level
            //Display "Level done, ready up for next" UI dialog
            if (currentLevel < levels.Count - 1)
                UIManager.instance.BannerUIText(
                    string.Format("Press {0} to continue to the next level", onReadyUpButtonName));
            else
                UIManager.instance.BannerUIText(
                    string.Format("Press {0} to finish", onReadyUpButtonName));
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName));

        }

        //Display "Game Finish!" UI Dialog
        UIManager.instance.BannerUIText(
                    string.Format(GameWinner()));

    }

    private void ActivateBlockEffects() {
        blockEffectsEnabled = true;
        foreach (BlockBase block in blocks) {
            if (block.blockEffect) {
                block.blockEffect.SetActive(true);
            }
        }
    }

    private void ActivatePlayers() {
        for (int i = 0; i < players.Count; i++)
            players[i].gameObject.SetActive(true);
        ResetPlayers();
    }

    private void MovePlayersTo(Transform pos) {
        for (int i = 0; i < players.Count; i++)
            players[i].transform.position = pos.position;
        ResetPlayers();
    }

    private void DeactivatePlayers() {
        for (int i = 0; i < players.Count; i++)
            players[i].gameObject.SetActive(false);
        ResetPlayers();
    }

    private string GameWinner() {
        int index = -1;
        for (int i = 0; i < players.Count - 1; i++) {
            if (players[i].score > players[i + 1].score)
            {
                index = i;
            }
            else if (players[i].score == players[i + 1].score)
            {
                return string.Format("Both Players Tie");
            }
        }
        return string.Format("Player {0} wins the game!", ++index);
    }

    private void ResetPlayers () {
        for (int i = 0; i < players.Count; i++)
            players[i].movement.Die();
    }

    private void SettleScore() {

    }

}