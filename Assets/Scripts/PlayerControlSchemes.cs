using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControlSchemes", menuName = "Player Control Schemes", order = 1)]
public class PlayerControlSchemes : ScriptableObject
{
    public string[,] playerControlAxes = new string[,]
    {
        { "Horizontal", "Vertical" },
        { "Horizontal2", "Vertical2" },
        { "Horizontal3", "Vertical3" },
        { "Horizontal4", "Vertical4" }
    };
    
}
