using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (CanvasGroup))]
public class TooltipUI : MonoBehaviour {

    public static TooltipUI instance { get; private set; }

    public TooltippedButton currentButton;
    public TMPro.TextMeshProUGUI text;
    public RectTransform arrowPointer;
    public float arrowLerpSpeed;
    public float appearSpeed;

    public CanvasGroup group { get; private set; }

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {

        group = GetComponent<CanvasGroup>();

    }

    // Update is called once per frame
    void Update() {

        if (currentButton) {
            text.text = currentButton.tooltipInfo;
            arrowPointer.transform.position = Vector3.Lerp(arrowPointer.transform.position, new Vector3(currentButton.transform.position.x, arrowPointer.transform.position.y), Time.deltaTime * arrowLerpSpeed);
            group.alpha = Mathf.Min(group.alpha + (appearSpeed * Time.deltaTime), 1);
        } else
            group.alpha = 0;

    }

}