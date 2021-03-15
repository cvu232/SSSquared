using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameOptions : MonoBehaviour
{
    public static GameOptions instance;

    public GameObject optionsCanvas;
    public GameObject optionsTexts;
    public GameObject optionsFields;

    public float d_scoreBase;
    public float d_scoreMult;
    public float d_deathScorePenalty;
    public float d_timeScoreMult;
    public float d_builderScoreMult;
    public float d_badLevelScorePenalty;

    public float scoreBase;
    public float scoreMult;
    public float deathScorePenalty;
    public float timeScoreMult;
    public float builderScoreMult;
    public float badLevelScorePenalty;

    public TextMeshProUGUI Option1TMP;
    public TextMeshProUGUI Option2TMP;
    public TextMeshProUGUI Option3TMP;
    public TextMeshProUGUI Option4TMP;
    public TextMeshProUGUI Option5TMP;
    public TextMeshProUGUI Option6TMP;

    public TMP_InputField Option1Field;
    public TMP_InputField Option2Field;
    public TMP_InputField Option3Field;
    public TMP_InputField Option4Field;
    public TMP_InputField Option5Field;
    public TMP_InputField Option6Field;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Get Options Canvas //
        optionsCanvas = GameObject.Find("GameOptionsCanvas");

        // Texts for Options //
        optionsTexts = optionsCanvas.transform.GetChild(1).gameObject;

        // Fields for Options //
        optionsFields = optionsCanvas.transform.GetChild(2).gameObject;

        // Default Score Options //
        d_scoreBase = 100;
        d_scoreMult = 2;
        d_deathScorePenalty = 0;
        d_timeScoreMult = 1;
        d_builderScoreMult = 2;
        d_badLevelScorePenalty = 3;

        SetDefaultScoreOptions();

        // Get TMP text component //
        Option1TMP = optionsTexts.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Option2TMP = optionsTexts.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Option3TMP = optionsTexts.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Option4TMP = optionsTexts.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Option5TMP = optionsTexts.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        Option6TMP = optionsTexts.transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        // Get InputField component //
        Option1Field = optionsFields.transform.GetChild(0).GetComponent<TMP_InputField>();
        Option2Field = optionsFields.transform.GetChild(1).GetComponent<TMP_InputField>();
        Option3Field = optionsFields.transform.GetChild(2).GetComponent<TMP_InputField>();
        Option4Field = optionsFields.transform.GetChild(3).GetComponent<TMP_InputField>();
        Option5Field = optionsFields.transform.GetChild(4).GetComponent<TMP_InputField>();
        Option6Field = optionsFields.transform.GetChild(5).GetComponent<TMP_InputField>();

        // Set Option text //
        Option1TMP.text = String.Format("{0,-5} : {1}", "Score Base", scoreBase);
        Option2TMP.text = String.Format("{0,-5} : {1}", "Score Multiplier", scoreMult);
        Option3TMP.text = String.Format("{0,-5} : {1}", "Death Score Penalty", deathScorePenalty);
        Option4TMP.text = String.Format("{0,-5} : {1}", "Time Score Mult", timeScoreMult);
        Option5TMP.text = String.Format("{0,-5} : {1}", "Builder Score Mult", builderScoreMult);
        Option6TMP.text = String.Format("{0,-5} : {1}", "Bad Level Score Penalty", badLevelScorePenalty);

        // Add Listener //
        Option1Field.onEndEdit.AddListener(delegate { SetScoreBase(Option1Field); });
        Option2Field.onEndEdit.AddListener(delegate { SetScoreMult(Option2Field); });
        Option3Field.onEndEdit.AddListener(delegate { SetDeathScorePenalty(Option3Field); });
        Option4Field.onEndEdit.AddListener(delegate { SetTimeScoreMult(Option4Field); });
        Option5Field.onEndEdit.AddListener(delegate { SetBuilderScoreMult(Option5Field); });
        Option6Field.onEndEdit.AddListener(delegate { SetBadLevelScorePenalty(Option6Field); });

        // Set initial InputField text //
        Option1Field.text = string.Format(scoreBase.ToString());
        Option2Field.text = string.Format(scoreMult.ToString());
        Option3Field.text = string.Format(deathScorePenalty.ToString());
        Option4Field.text = string.Format(timeScoreMult.ToString());
        Option5Field.text = string.Format(builderScoreMult.ToString());
        Option6Field.text = string.Format(badLevelScorePenalty.ToString());
    }

    private void SetDefaultScoreOptions()
    {
        scoreBase = d_scoreBase;
        scoreMult = d_scoreMult;
        deathScorePenalty = d_deathScorePenalty;
        timeScoreMult = d_timeScoreMult;
        builderScoreMult = d_builderScoreMult;
        badLevelScorePenalty = d_badLevelScorePenalty;
    }

    private void SetScoreBase(TMP_InputField field)
    {
        scoreBase = float.Parse(field.text) < 0 ? 0 : float.Parse(field.text);
        field.text = scoreBase.ToString();
        Option1TMP.text = String.Format("{0,-5} : {1}", "Score Base", scoreBase);
    }

    private void SetScoreMult(TMP_InputField field)
    {
        scoreMult = float.Parse(field.text) < 0 ? 0 : float.Parse(field.text);
        field.text = scoreMult.ToString();
        Option2TMP.text = String.Format("{0,-5} : {1}", "Score Multiplier", scoreMult);
    }

    private void SetDeathScorePenalty(TMP_InputField field)
    {
        deathScorePenalty = float.Parse(field.text) < 0 ? 0 : float.Parse(field.text);
        field.text = deathScorePenalty.ToString();
        Option3TMP.text = String.Format("{0,-5} : {1}", "Death Score Penalty", deathScorePenalty);
    }

    private void SetTimeScoreMult(TMP_InputField field)
    {
        timeScoreMult = float.Parse(field.text) < 0 ? 0 : float.Parse(field.text);
        field.text = timeScoreMult.ToString();
        Option4TMP.text = String.Format("{0,-5} : {1}", "Time Score Mult", timeScoreMult);
    }

    private void SetBuilderScoreMult(TMP_InputField field)
    {
        builderScoreMult = float.Parse(field.text) < 0 ? 0 : float.Parse(field.text);
        field.text = builderScoreMult.ToString();
        Option5TMP.text = String.Format("{0,-5} : {1}", "Builder Score Mult", builderScoreMult);
    }
    
    private void SetBadLevelScorePenalty(TMP_InputField field)
    {
        badLevelScorePenalty = float.Parse(field.text) < 0 ? 0 : float.Parse(field.text);
        field.text = badLevelScorePenalty.ToString();
        Option6TMP.text = String.Format("{0,-5} : {1}", "Bad Level Score Penalty", badLevelScorePenalty);
    }
}
