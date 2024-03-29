﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;

    public ScoreConfig scoreConfig;

    private GameOptions gameOptions;
    private GamePhaseManager gpm;
    private List<Player> winners;

    private void Awake() {
        if (instance && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    private void Start() {
        gameOptions = GameOptions.instance;
        //If no score config asset was specified, look for the default inside "/Resources/DefaultScoreConfig.asset"
        if (!scoreConfig)
            try {
                scoreConfig = (ScoreConfig) Resources.Load(ScoreConfig.DEFAULT_FILENAME);
            } catch { }
    }

    // Settles Score of each Player after each Level. Adjust Score formula here //
    public void SettleLevelScores(Level level) {

        //Find the gpm instance every time the score is calculated, in case the scene changes and the referenced instanced is destroyed
        gpm = GamePhaseManager.instance;

        for (int i = 0; i < gpm.players.Count; i++) {
            // if this player built a bad level (is builder && no winner)
            if (gameOptions && gameOptions.deathScorePenalty && level.builder == gpm.players[i] && !level.winner) {
                gpm.players[i].score -= scoreConfig.badLevelScorePenaltyBonus * scoreConfig.scoreBaseMulti;
            }
            // if this player built the level and there is a winner (is builder && any winner)
            if (level.builder == gpm.players[i] && level.winner) {
                gpm.players[i].score += scoreConfig.builderLevelCompletedBonus * scoreConfig.scoreBaseMulti;
            }
            // if this player won (is winner)
            if (level.winner == gpm.players[i]) {
                gpm.players[i].score += scoreConfig.winnerBonus * scoreConfig.scoreBaseMulti;
            }
            //Debug.Log(string.Format("Player {0},{1} with {2}", gpm.players[i].playerID, gpm.players[i].character, gpm.players[i].score));
        }
    }

    public void SettleGameWinner() {

        //Erase the winner array every time the winner is calculated, otherwise, the list will be populated with null items
        winners = new List<Player>();

        //Start the best score at the first player's score (in case all scores are less than 0
        float bestScore = gpm.players[0].score;

        for (int i = 0; i < gpm.players.Count; i++) {
            //If this player's score surpassed all other scores, clear tie list
            if (gpm.players[i].score > bestScore)
            {
                winners.Clear(); // Clear all winners
                bestScore = gpm.players[i].score; // Set new best
                winners.Add(gpm.players[i]); // Add player as winner
            }
            else if (gpm.players[i].score == bestScore) // If equal best
            {
                winners.Add(gpm.players[i]); // Add new winner
            }
        }

        if (winners.Count != 1) {
            UIManager.instance.BannerUIText(string.Format
                ("{0}-way tie!", winners.Count));
        } else if (winners.Count == 1)
            UIManager.instance.BannerUIText(string.Format
                ("{0} wins!", winners[0].character));

    }

}