using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int playerID;
    public Characters character;
    public int score;
    public int HP;
    public bool isReady;
    public PlayerMovement movement { get; private set; }
    public PlayerAudio audioSource { get; private set; }
    public SpriteRenderer renderer;
    public UnityEngine.UI.Image playerIndicator;

    public PlayerControlSchemes controls; // Set in inspector

    private void Awake() {
        score = 0;
        isReady = false;
        movement = GetComponent<PlayerMovement>();
        controls = Resources.Load<PlayerControlSchemes>("PlayerControlSchemes");
        audioSource = GetComponent<PlayerAudio>();
    }

    public void Ready() {
        isReady = true;
    }

    public void unReady() {
        isReady = false;
    }

    public void resetHP() {

    }

    public override string ToString() {
        return character.ToString();
    }

    public void AssignPlayer(int i) {
        Debug.Log(controls);
        //movement.inputHorizontalAxisName = "Horizontal" + (playerID == 0 ? "" : (playerID + 1).ToString());
        //movement.inputVerticalAxisName = "Vertical" + (playerID == 0 ? "" : (playerID + 1).ToString());
        playerID = i;
        movement.inputHorizontalAxisName = controls.playerControlAxes[i, 0];
        movement.inputVerticalAxisName = controls.playerControlAxes[i, 1];
        renderer.material = GameOptions.instance.characterSpriteOutlineMaterials[i];
        playerIndicator.sprite = GameOptions.instance.playerIndexIndicatorSprites[i];
        playerIndicator.color = GameOptions.instance.playerColours[i];
    }

}