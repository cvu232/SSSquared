using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : MonoBehaviour
{
    public GameObject levelsContainer; // object that holds all levels
    public GameObject playersContainer; // object that holds all players
    public Canvas canvas; // canvas for UI
    public static Canvas glCanvas;
    public GameObject buildersCanvas;

    public static bool readyCheck; // ready check before starting game

    public static bool isBuildModeOn;
    public static int BuildTimerInit = 60; // initial build mode time in seconds
    public static int BuildTimer; // build mode time in seconds

    public static bool isRaceModeOn;
    public static int RaceTimerInit = 150; // initial platform mode time in seconds
    public static int RaceTimer; // platform mode time in seconds

    public static bool BlockEffectsEnabled; // are block effects active

    public static int prog;  // current level

    public static List<Level> levels;
    public static List<Player> players;

    public static BlockBase[] blocks;

    public static Level currentLevel;
    public static Player currentBuilder;

    private void Start()
    {
        readyCheck = false;
        BuildTimer = BuildTimerInit;
        RaceTimer = RaceTimerInit;
        BlockEffectsEnabled = false;

        prog = 0;

        buildersCanvas = canvas.gameObject;
        buildersCanvas.SetActive(true);

        foreach (GameObject o in playersContainer.transform)
        {
            players.Add(o.GetComponent<Player>());
        }
        foreach (GameObject o in levelsContainer.transform)
        {
            levels.Add(o.GetComponent<Level>());
        }

        // player 1 is set as builder
        currentBuilder = players[0];
        // start at level 0
        currentLevel = levels[0];

        // players ready check
        StartCoroutine(ReadyCheck());
    }

    private void Update()
    {

    }

    IEnumerator ReadyCheck()
    {
        yield return new WaitUntil(() => readyCheck);
        StartCoroutine(BuildMode());
    }

    IEnumerator BuildMode()
    {
        // enable builder's canvas
        buildersCanvas.SetActive(true);
        yield return new WaitForSeconds(BuildTimer);
        MakeArrayOfBlocks();
        buildersCanvas.SetActive(false);
        EndBuildMode();
        yield return new WaitUntil(() => !isBuildModeOn);
        StartCoroutine(RaceMode());
    }

    IEnumerator RaceMode()
    {
        ActivateBlockEffects();
        yield return new WaitForSeconds(RaceTimer);
        // <- settle level
        EndRaceMode();
        yield return new WaitUntil(() => !isRaceModeOn);
        currentBuilder = players[++prog];
        currentLevel = levels[prog];
        // <- instatiate new level
        StartCoroutine(BuildMode());
    }

    private void EndBuildMode()
    {
        isBuildModeOn = false;
    }

    private void EndRaceMode()
    {
        isRaceModeOn = false;
    }

    private void MakeArrayOfBlocks()
    {
        blocks = FindObjectsOfType<BlockBase>();
    }

    private void ActivateBlockEffects()
    {
        BlockEffectsEnabled = true;
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
        BlockEffectsEnabled = false;
        foreach (BlockBase block in blocks)
        {
            block.delete();
        }
    }

    /*
    IEnumerator SettleScores()
    {

    }
    */
}
