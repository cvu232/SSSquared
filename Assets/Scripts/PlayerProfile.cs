using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile : MonoBehaviour {

    public int activePlayer;

    public Image bg;
    public Color[] BGColors;
    public GameObject[] profiles;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        bg.color = Color.Lerp(bg.color, BGColors[activePlayer], Time.deltaTime * 5);

        for (int i = 0; i < profiles.Length; i++) {

            profiles[i].SetActive(i == activePlayer);

        }

    }

}