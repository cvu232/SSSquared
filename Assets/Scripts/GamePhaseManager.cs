using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Phases {
    buildPhase,
    racePhase
}

public class GamePhaseManager : MonoBehaviour
{
    public static GamePhaseManager instance;

    public static Phases currentGamePhase { get; private set; }

    [Header("Container Objects")]
    public GameObject levelsContainer; // object that holds all levels
    public GameObject playersContainer; // object that holds all players
    [Header("Canvas Objects")]
    public GameObject bannerCanvas;
    public GameObject scoreCanvas;
    public GameObject timerCanvas;
    public GameObject buildersCanvas;

    [Header("Build Phase Parameters")]
    public float buildPhaseDuration = 60; // initial build mode time in seconds
    [Tooltip("What key should be pressed to start the next player's build turn")]
    public string onReadyUpButtonName;
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
        if (instance != null)
            Destroy(instance);
        else
            instance = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Initialize some variables //
        blockEffectsEnabled = false;

        currentLevel = 0;

        buildersCanvas.SetActive(false);
        scoreCanvas.SetActive(false);
        timerCanvas.SetActive(false);

        //Placeholder players
        players.Add(player1);
        players.Add(player2);
        DeactivatePlayers();

        // If players should have a fixed number of levels to build, override the maxLevelCount variable here
        if (useFixedLevelsPerPlayer)
            maxLevelCount = players.Count * fixedLevelsPerPlayer;
        else
            fixedLevelsPerPlayer = -1;

        //Generate levels and add them to 'levels' array
        for (int i = 0; i < maxLevelCount; i++)
        {
            Level newLevel = Instantiate(emptyLevelPrefab).GetComponent<Level>();
            newLevel.transform.parent = levelsContainer.transform;
            newLevel.transform.localPosition = Vector3.up * (i * emptyLevelSpacing);
            levels.Add(newLevel);
        }

        StartCoroutine(BuildPhase());
    }

    private void Update()
    {
        currentPhaseTimer -= Time.deltaTime;
        if (timerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>() && currentPhaseTimer >= 0)
            timerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Remaining Time : {0:F2}", currentPhaseTimer);
    }

    IEnumerator BuildPhase()
    {
        //Setup for Build Phase
        currentGamePhase = Phases.buildPhase; // Set current phase
        bannerCanvas.SetActive(true); // Enable top canvas

        for (currentLevel = 0, currentBuilder = 0; currentLevel < levels.Count; currentLevel++, currentBuilder++)
        {
            //Initialize current player's build phase
            //Move Camera
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, currentLevel * emptyLevelSpacing, Camera.main.transform.position.z);
            //Set UI to indicate current builder player
            bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "Player " + (currentBuilder + 1) + " is building Level " + (currentLevel + 1);

            //UI to indicate which button to start
            bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    "Player " + (currentBuilder + 1) + " press " + onReadyUpButtonName + " to begin building your level";
            // Wait for Player to be ready to build
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName));
            buildersCanvas.SetActive(true); // Enable builder's canvas
            timerCanvas.SetActive(true);
            currentPhaseTimer = buildPhaseDuration; // Start build timer

            //Wait for Build Phase
            yield return new WaitUntil(() => currentPhaseTimer <= 0); // Wait until building finishes
            currentPhaseTimer = 0;
            //End current player's build phase
            buildersCanvas.GetComponentInChildren<BuilderController>().DeactivateBuilderCanvas();

            //Increase the current player index, loop to start if index gets out of bounds
            if (currentBuilder >= players.Count)
                currentBuilder = 0;
            levels[currentBuilder].builderIndex = currentBuilder;
            
            //Acknowledge next player to build or build phase is complete
            if (currentLevel < levels.Count - 1)
                bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    "Press " + onReadyUpButtonName + " proceed to building the next level";
            else
                bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    "Press " + onReadyUpButtonName + " to proceed to the Racing Phase";
        }

        //Start Next Phase
        StartCoroutine(RacePhase()); // Start race mode
    }

    IEnumerator RacePhase()
    {
        //Setup for Race Phase
        currentGamePhase = Phases.racePhase; // Set current phase
        ActivateBlockEffects(); // Enable block effects
        //TODO: Generate array of ready status booleans

        for (currentLevel = 0; currentLevel < levels.Count; currentLevel++)
        {
            //Move camera and all players to current level's start area
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, currentLevel * emptyLevelSpacing, Camera.main.transform.position.z);
            MovePlayersTo(levels[currentLevel].spawnPoint);
            scoreCanvas.SetActive(true);

            //Set UI to indicate current level and its builder
            bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "Level: " + (currentLevel + 1) + ". Built by Player " + levels[currentLevel].builderIndex;
            scoreCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "Score: " + players[0].score + ":" + players[1].score;

            //Wait for all players to be ready
            bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    "Press " + onReadyUpButtonName + " begin racing";
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName)); // Wait until next player presses 'ready' button
            ActivatePlayers();
            currentPhaseTimer = racePhaseDuration; // Set build timer

            //Wait for Level Timeout
            //TODO end current level loop if a player beats the level
            yield return new WaitUntil(() => currentPhaseTimer <= 0); // Wait until race finishes
            currentPhaseTimer = 0;
            DeactivatePlayers();


            if (!levels[currentLevel].winner)
            {
                players[levels[currentLevel].builderIndex].score -= 3;
                bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "There was no winner for this level... Player " + (levels[currentLevel].builderIndex + 1) + " suffers a score penalty.";
            }
            else if (levels[currentLevel].winner)
            {
                levels[currentLevel].winner.score += 3;
                bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "Player " + (players.IndexOf(levels[currentLevel].winner) + 1) + " is the winner of this level!";
                
            }

            //End current level
            //Display "Level done, ready up for next" UI dialog
            if (currentLevel < levels.Count - 1)
                bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "Press " + onReadyUpButtonName + " to continue to the next level";
            else
                bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "Press " + onReadyUpButtonName + " to finish";
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName));

        }

        //Display "Game Finish!" UI Dialog
        bannerCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                "Player "+ SettledWinner() + " wins the game!";

    }

    private void ActivateBlockEffects()
    {
        blockEffectsEnabled = true;
        foreach (BlockBase block in blocks)
        {
            if (block.blockEffect)
            {
                block.blockEffect.SetActive(true);
            }
        }
    }

    private void ActivatePlayers()
    {
        for (int i = 0; i < players.Count; i++)
            players[i].gameObject.SetActive(true);
    }

    private void MovePlayersTo(Transform pos)
    {
        for (int i = 0; i < players.Count; i++)
            players[i].transform.position = pos.position;
    }

    private void DeactivatePlayers()
    {
        for (int i = 0; i < players.Count; i++)
            players[i].gameObject.SetActive(false);
    }

    private int SettledWinner()
    {
        int index = -1;
        for (int i = 0; i < players.Count - 1; i++)
        {
            if (players[i].score >= players[i + 1].score)
                index = i;
        }
        return index + 1;
    }

    private void SettleScore()
    {

    }
}
