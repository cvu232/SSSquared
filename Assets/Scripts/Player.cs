using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int score;
    public int HP;

    private void Start()
    {
        score = 0;
    }

    public void resetHP()
    {
        
    }

    public void settlePoints(int timeElapsed)
    {
        score += timeElapsed * 100;
    }

}