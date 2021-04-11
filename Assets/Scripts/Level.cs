using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Level : MonoBehaviour {

    public Transform spawnPoint;
    public Transform endPoint;
    public int builderIndex;
    public float killHeight;
    public Player winner;
    public int portalPairLimit = 4; // + 1 for real number
    public int portalPairs;

    private void Update() {
        Debug.DrawLine(Vector3.up * killHeight + Vector3.left * 50, Vector3.up * killHeight + Vector3.right * 50, Color.red);
    }

}