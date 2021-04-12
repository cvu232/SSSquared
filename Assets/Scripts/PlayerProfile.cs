using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile : MonoBehaviour {

    public int activePlayer;
    public Image bg;
    public GameObject[] profiles;

    private Color[] BGColors;

    // Start is called before the first frame update
    void Start() {

        BGColors = GameOptions.instance.playerColours;

    }

    // Update is called once per frame
    void Update() {

        bg.color = Color.Lerp(bg.color, BGColors[activePlayer], Time.deltaTime * 5);

        for (int i = 0; i < profiles.Length; i++) {

            profiles[i].SetActive(i == (int)GameOptions.instance.charactersPerPlayer[activePlayer]);

        }

    }

}