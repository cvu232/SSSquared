using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float score;
    public int HP;
    public bool isReady;
    public PlayerMovement movement { get; private set; }

    private void Awake()
    {
        score = 0;
        isReady = false;
        movement = GetComponent<PlayerMovement>();
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
}