using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultScoreConfig", menuName = "Score Configuration", order = 1)]
public class ScoreConfig : ScriptableObject {

    public static readonly string DEFAULT_FILENAME = "DefaultScoreConfig";

    public int scoreBaseMulti;
    public int badLevelScorePenaltyBonus;
    public int builderLevelCompletedBonus;
    public int winnerBonus;

}