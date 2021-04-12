using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreenItem : MonoBehaviour {

    public Image image;
    public TextMeshProUGUI label;

    private Image outline;

    // Start is called before the first frame update
    void Start() {

        outline = GetComponent<Image>();

    }

    // Update is called once per frame
    void Update() {

    }

    public void Set (Sprite sprite, string name, Color color) {

        image.sprite = sprite;
        label.text = name;
        outline.color = color;

    }

}