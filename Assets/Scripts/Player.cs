using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int score;
    public int HP;
    public bool isReady;

    private void Start()
    {
        score = 0;
        isReady = false;
    }

    public Transform transform
    {
        get { return transform; }
        set { transform = value;  }
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