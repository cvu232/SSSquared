using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public string charName;
    public int score;
    public int HP;
    public bool isReady;
    public PlayerMovement movement { get; private set; }
    public PlayerAudio audioSource { get; private set; }

    public PlayerControlSchemes controls; // Set in inspector

    private void Awake()
    {
        score = 0;
        isReady = false;
        movement = GetComponent<PlayerMovement>();
        controls = Resources.Load<PlayerControlSchemes>("PlayerControlSchemes");
        audioSource = GetComponent<PlayerAudio>();
    }

    public void Ready()
    {
        isReady = true;
    }

    public void unReady()
    {
        isReady = false;
    }

    public void resetHP()
    {
        
    }

    public override string ToString() {
        return charName;
    }
    public void assignControls(int i)
    {
        Debug.Log(controls);
        movement.inputHorizontalAxisName = controls.playerControlAxes[i, 0];
        movement.inputVerticalAxisName = controls.playerControlAxes[i, 1];
    }

}