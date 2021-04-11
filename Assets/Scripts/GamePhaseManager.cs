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

    [Header("Misc. Parameters")]
    public float currentPhaseTimer; // initial platform mode time in seconds

    public List<Level> levels;
    public List<Player> players;

    public BuildableObject[] blocks;

    public PlayerProfile playerProfile;
    public int currentLevel;
    public int currentBuilder;


    private void Awake() // Singleton
    {
        if (instance != null && instance != this)
            Destroy(this);
        else {

            instance = this;

        }

    }

    private void Start() {

        InitialSetup();

    }

    private void InitialSetup() {

        //Make sure there is a GameOptions instance available
        if (!GameOptions.instance)
            Instantiate((GameObject)Resources.Load("Prefabs/GameOptions")).GetComponent<GameOptions>().Awake();

        useFixedLevelsPerPlayer = false;
        maxLevelCount = GameOptions.instance.levelCount;

        // Initialize some variables //
        Physics2D.queriesStartInColliders = true;


        UIManager.instance.uiBuildBar.Initial();

        levelsContainer = GameObject.Find("LevelsCONTAINER");
        playersContainer = GameObject.Find("PlayersCONTAINER");

        Time.timeScale = UIManager.runningTimeScale;

        currentGamePhase = Phases.buildPhase;
        currentLevel = 0;
        currentBuilder = 0;

        // Get profile of active player
        playerProfile = FindObjectOfType<PlayerProfile>();

        // List all Players in game
        players = new List<Player>();
        foreach (Player p in FindObjectsOfType<Player>()) {
            Debug.Log("Found " + p.name);
            if (players.Count < GameOptions.instance.playerCount)
            {
                p.assignControls(players.Count);
                Debug.Log("Assigned controller " + players.Count + " to " + p.name);
                players.Add(p);
            }
            else
            {
                Debug.Log("Destroying " + p.name + " as it is over the player cap");
                Destroy(p.gameObject);
            }
        }

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

            //loop to start if index gets out of bounds
            if (currentBuilder >= players.Count)
                currentBuilder = 0;
            levels[currentBuilder].builderIndex = currentBuilder;
            playerProfile.activePlayer = levels[currentBuilder].builderIndex = currentBuilder;

            //Initialize current player's build phase

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
        //ActivateBlockEffects(); // Enable block effects
        //TODO: Generate array of ready status booleans

        for (currentLevel = 0; currentLevel < levels.Count; currentLevel++) {
            //Move camera and all players to current level's start area
            //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, currentLevel * emptyLevelSpacing, Camera.main.transform.position.z);

            MovePlayersTo(levels[currentLevel].spawnPoint);
            playerProfile.activePlayer = levels[currentLevel].builderIndex;

            //Wait for all players to be ready
            UIManager.instance.BannerUIText(
                string.Format("Press {0} begin racing", onReadyUpButtonName));
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName)); // Wait until next player presses 'ready' button
            //Set UI to indicate current level and its builder
            UIManager.instance.BannerUIText(
                string.Format("Level: {0}. Built by Player {1} ", (currentLevel + 1), levels[currentLevel].builderIndex + 1));
            ActivatePlayers();
            currentPhaseTimer = racePhaseDuration; // Set build timer

            //Wait for Level Timeout
            yield return new WaitUntil(() => currentPhaseTimer <= 0.001f); // Wait until race finishes
            currentPhaseTimer = 0;
            DeactivatePlayers();

            // Settle scores for this level
            ScoreManager.instance.SettleLevelScores(levels[currentLevel]);
            if (!levels[currentLevel].winner) {
                UIManager.instance.BannerUIText(
                    string.Format("There was no winner for this level... Player {0} suffers a score penalty.", levels[currentLevel].builderIndex + 1));
            } else if (levels[currentLevel].winner) {
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
            
            StartCoroutine(DisableLevelAfter(levels[currentLevel], 2.0f)); // Disable level after player prompts

        }

        //Display "Game Finish!" UI Dialog
        ScoreManager.instance.SettleGameWinner();

    }

    IEnumerator DisableLevelAfter(Level level, float f)
    {
        yield return new WaitForSeconds(f);
        level.gameObject.SetActive(false);
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

    private void ResetPlayers () {
        for (int i = 0; i < players.Count; i++)
            players[i].movement.Die();
    }
}