using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    GameOptions score;
    GamePhaseManager GPM;

    List<Player> winners;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        winners = new List<Player>();
        GPM = GamePhaseManager.instance;
        score = GameOptions.instance;
    }

    // Settles Score of each Player after each Level. Adjust Score formula here //
    public void SettleLevelScores(Level level)
    {
        for ( int i = 0; i < GPM.players.Count; i++)
        {
            // if this player built a bad level (no winner)
            if (GPM.players[level.builderIndex] == GPM.players[i] && !level.winner)
            {
                GPM.players[i].score -= score.badLevelScorePenalty * score.scoreBase * score.scoreMult;
            }
            // if this play didn't build this level and did not win (!builder = !winner)
            else if (GPM.players[level.builderIndex] != GPM.players[i] && level.winner != GPM.players[i])
            {
                
            }
            // if this player built a level and won the level (builder = winner)
            else if (GPM.players[level.builderIndex] == GPM.players[i] && level.winner == GPM.players[i])
            {
                GPM.players[i].score += score.scoreMult * score.scoreBase / score.builderScoreMult;
            }
            // if this player built a level and did not win (builder = !winner)
            else if (GPM.players[level.builderIndex] == GPM.players[i] && level.winner != GPM.players[i])
            {
                GPM.players[i].score += score.scoreBase * score.builderScoreMult;
            }
            // if this player didn't build this level and won (!builder = winner)
            else if (GPM.players[level.builderIndex] != GPM.players[i] && level.winner == GPM.players[i])
            {
                GPM.players[i].score += score.scoreMult * score.scoreBase;
            }
        }
    }

    public void SettleGameWinner()
    {
        float recordScore = 0;
        //string final = "Player ";

        for (int i = 0; i < GPM.players.Count; i++)
        {
            // if this player has the highest score
            if (GPM.players[i].score > recordScore)
            {
                // set this as new highest score
                recordScore = GPM.players[i].score;
                winners.Clear(); // clear all winners
                winners.Add(GPM.players[i]); // add new winner
            }
            // if this player has the highest score
            else if (GPM.players[i].score == recordScore)
            {
                // add player to winners
                winners.Add(GPM.players[i]);
            }
        }

        /*
        if (winners.Count < GPM.players.Count)
        {
            for (int i = 0; i < winners.Count; i++)
            {
                final += string.Format("{0}", i);
            }
        }
        */
        if (winners.Count == GPM.players.Count)
        {
            UIManager.instance.BannerUIText(
                    string.Format("Players all tie!"));
        }
        else
            UIManager.instance.BannerUIText(
                    string.Format("Player {0} wins!"));


    }
}
