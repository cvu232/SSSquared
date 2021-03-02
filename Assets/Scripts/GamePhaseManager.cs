using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phases {
    buildPhase,
    racePhase
}

public class GamePhaseManager : MonoBehaviour
{
    public static GamePhaseManager instance;

    public static Phases currentGamePhase { get; private set; }

    //Public Variables
    public GameObject levelsContainer; // object that holds all levels
    public GameObject playersContainer; // object that holds all players
    public Canvas canvas; // canvas for UI
    public static Canvas glCanvas;
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

    [Header("Misc. Parameters")]
    public float currentPhaseTimer = 150; // initial platform mode time in seconds

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

        buildersCanvas = canvas.gameObject;
        buildersCanvas.SetActive(true);

        foreach (GameObject o in playersContainer.transform)
        {
            players.Add(o.GetComponent<Player>());
        }

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

        // Players Ready check
        //StartCoroutine(ReadyCheck());
        StartCoroutine(BuildPhase());
    }

    private void Update()
    {
        currentPhaseTimer -= Time.deltaTime;
    }

    IEnumerator BuildPhase()
    {
        //Setup for Build Phase
        currentGamePhase = Phases.buildPhase; // Set current phase
        buildersCanvas.SetActive(true); // Enable builder's canvas

        for (currentLevel = 0; currentLevel < levels.Count; currentLevel++)
        {

            //Initialize current player's build phase
            //TODO: Set UI to indicate curent builder player
            currentPhaseTimer = buildPhaseDuration; // Set build timer

            //Increase the current player index, loop to start if index gets out of bounds
            currentBuilder++;
            if (currentBuilder >= players.Count)
                currentBuilder -= players.Count;
            levels[currentBuilder].builderIndex = currentBuilder;

            //Wait for Build Phase
            yield return new WaitUntil(() => currentPhaseTimer <= 0); // Wait until building finishes

            //End current player's build phase
            //TODO: Display "Waiting for next player" UI dialog
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName)); // Wait until next player presses 'ready' button

        }

        //End Build Phase
        MakeArrayOfBlocks(); // catalog the blocks in the scene
        buildersCanvas.SetActive(false); // Disable builder canvas

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

            //Initialize current player's build phase
            //TODO: Move all players to current level's start area
            //TODO: Set UI to indicate curent builder player
            currentPhaseTimer = racePhaseDuration; // Set build timer

            //Wait for Level Timeout
            //TODO end current level loop if a player beats the level
            yield return new WaitUntil(() => currentPhaseTimer <= 0); // Wait until building finishes
            //TODO add code to add score penalty if level timeout is reached

            //End current level
            //TODO: Display "Level done, ready up for next" UI dialog
            yield return new WaitUntil(() => Input.GetButtonDown(onReadyUpButtonName)); // Wait until next player presses 'ready' button

        }
        
        //Display "Game Finish!" UI Dialog

    }

    private void MakeArrayOfBlocks()
    {
        blocks = FindObjectsOfType<BlockBase>();
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
    private void ClearArrayOfBlocks()
    {
        blockEffectsEnabled = false;
        foreach (BlockBase block in blocks)
        {
            block.delete();
        }
    }

    /*
    private void SettleScore()
    {

    }
    */
}
