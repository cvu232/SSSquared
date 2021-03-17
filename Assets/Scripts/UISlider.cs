using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Slider))]
public class UISlider : MonoBehaviour {

    public TMPro.TextMeshProUGUI handleLabel;
    private Slider slider;

    private void Start() {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update() {

        if (handleLabel)
            handleLabel.text = Mathf.RoundToInt(slider.value).ToString();

    }

}