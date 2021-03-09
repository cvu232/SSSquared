using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public float lerpSpeed;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        Vector3 pos = GamePhaseManager.instance.levels[GamePhaseManager.instance.currentLevel].transform.position;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * lerpSpeed);

    }

}