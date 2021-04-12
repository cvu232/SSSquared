using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent (typeof (CanvasGroup))]
public class VictoryScreen : MonoBehaviour {

    public CanvasGroup[] resultsElements;

    [Header("Appear Attributes")]
    public TextMeshProUGUI noWinner;
    public TextMeshProUGUI noWinnerSubtitle;
    public TextMeshProUGUI title;
    public float startDelay;
    public float stagger;
    public float lerpSpeed;
    public Vector3 appearOffset;

    [Header("Sprites")]
    public Sprite[] spritesWin;
    public Sprite[] spritesLose;

    private Vector3[] originalPositions;
    private Vector3[] startPositions;
    private VictoryScreenItem[] resultItems;
    private CanvasGroup group;
    private UIPanel panel;
    private int winner;
    private bool _show;
    private bool[] showChildren;
    private float timer;

    // Start is called before the first frame update
    void Start() {

        group = GetComponent<CanvasGroup>();
        panel = GetComponent<UIPanel>();
        showChildren = new bool[resultsElements.Length];

        originalPositions = new Vector3[resultsElements.Length];
        startPositions = new Vector3[resultsElements.Length];

        resultItems = new VictoryScreenItem[resultsElements.Length];

        for (int i = 0; i < resultsElements.Length; i++) {
            originalPositions[i] = resultsElements[i].transform.localPosition;
            startPositions[i] = originalPositions[i] + appearOffset;
            resultsElements[i].alpha = 0;
            Debug.Log("getting", resultsElements[i]);
            resultItems[i] = resultsElements[i].GetComponent<VictoryScreenItem>();
        }

    }

    // Update is called once per frame
    void Update() {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Delete))
            if (panel.show)
                Hide();
            else
                Show(-1);
#endif

        if (!panel.show && _show)
            timer = 0;

        title.text = GamePhaseManager.instance.currentLevel == GamePhaseManager.instance.maxLevelCount - 1 ? ("Final Results") : ("Level #" + (GamePhaseManager.instance.currentLevel + 1) + " results");

        for (int i = 0; i < GameOptions.instance.playerCount; i++) {

            showChildren[i] = timer > i * stagger + startDelay && winner >= 0;

            if (showChildren[i]) {
                resultsElements[i].transform.localPosition = Vector3.Lerp(resultsElements[i].transform.localPosition, originalPositions[i], Time.deltaTime * lerpSpeed);
                resultsElements[i].alpha = Mathf.Lerp(resultsElements[i].alpha, 1, Time.deltaTime * lerpSpeed);
            } else {
                resultsElements[i].transform.localPosition = Vector3.Lerp(resultsElements[i].transform.localPosition, startPositions[i], Time.deltaTime * lerpSpeed);
                resultsElements[i].alpha = Mathf.Lerp(resultsElements[i].alpha, 0, Time.deltaTime * lerpSpeed);
            }

        }

        noWinner.alpha = Mathf.Lerp(noWinner.alpha, winner < 0 ? 1 : 0, Time.deltaTime * lerpSpeed);
        noWinnerSubtitle.alpha = noWinner.alpha;
        noWinnerSubtitle.text = "Points deducted from this level's creator: P" + (GamePhaseManager.instance.levels[GamePhaseManager.instance.currentLevel].builder.playerID + 1) + " (" + GameOptions.instance.charactersPerPlayer[GamePhaseManager.instance.levels[GamePhaseManager.instance.currentLevel].builder.playerID] + ")";

        group.alpha = Mathf.Lerp(group.alpha, panel.show ? 1 : 0, Time.deltaTime * lerpSpeed);

        _show = panel.show;
        if (panel.show)
            timer += Time.deltaTime;
        else
            timer = 0;

    }

    public void Show(int winningPlayer) {

        int p = 0;

        for (int i = 1; i < GameOptions.instance.playerCount; i++) {

            if (p == winningPlayer)
                p++;
            resultItems[i].Set(spritesLose[(int)GameOptions.instance.charactersPerPlayer[p]], GameOptions.instance.charactersPerPlayer[p].ToString(), GameOptions.instance.playerColours[p]);

            p++;

        }

        //Debug.Log(GameOptions.instance.charactersPerPlayer[winningPlayer]);

        if (winningPlayer >= 0)
            resultItems[0].Set(spritesWin[(int)GameOptions.instance.charactersPerPlayer [winningPlayer]], GameOptions.instance.charactersPerPlayer[winningPlayer].ToString(), GameOptions.instance.playerColours[winningPlayer]);

        winner = winningPlayer;
        panel.show = true;

    }

    public void Hide() {
        panel.show = false;
    }

}