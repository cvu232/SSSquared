using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CharSelectButtonStatus {

    regular,
    expanded,
    compact

}

public class CharSelectButton : UnityEngine.EventSystems.EventTrigger {

    public CharSelectButtonStatus status;
    public UnityEngine.UI.Image occupied;

    public float sizeSpeed;

    public float expandedLabelSize;
    public float compactLabelSize;
    public float regularWidth;
    public float expandedWidth;

    public int takenBy;
    [HideInInspector]
    public RectTransform playerIndicator;
    private CharSelect parent;
    private RectTransform transform;
    private TMPro.TextMeshProUGUI label;
    private float compactWidth;

    // Start is called before the first frame update
    void Start() {

        parent = GetComponentInParent<CharSelect>();
        transform = GetComponent<RectTransform>();
        label = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        compactWidth = (transform.parent.childCount * regularWidth - expandedWidth) / (transform.parent.childCount - 1);

        takenBy = -1;

        //GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 0.02f;

    }

    // Update is called once per frame
    void Update() {

        float width = 0;

        switch (status) {

            case CharSelectButtonStatus.regular:
                width = regularWidth;
                break;
            case CharSelectButtonStatus.compact:
                width = compactWidth;
                break;
            case CharSelectButtonStatus.expanded:
                width = expandedWidth;
                break;

        }

        Color col = takenBy < 0 ? Color.black : GameOptions.instance.playerColours[takenBy];
        col.a = Mathf.Lerp(occupied.color.a, takenBy != -1 ? 0.75f : 0, Time.deltaTime * 10);
        occupied.color = col;

        transform.sizeDelta = new Vector2(Mathf.Lerp(transform.sizeDelta.x, width, Time.deltaTime * sizeSpeed), transform.sizeDelta.y);
        label.fontSize = Mathf.Lerp(label.fontSize, status == CharSelectButtonStatus.compact ? compactLabelSize : expandedLabelSize, Time.deltaTime * sizeSpeed);

        if (takenBy == -1 && playerIndicator) {
            Destroy(playerIndicator.gameObject);
            playerIndicator = null;
        }

        if (playerIndicator)
            playerIndicator.position = Vector3.Lerp(playerIndicator.position, transform.position, Time.deltaTime * 10);

    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if (takenBy != -1)
            parent.selectedButton = null;
        parent.selectedButton = this;
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        if (parent.selectedButton == this)
            parent.selectedButton = null;
    }

    public override void OnPointerClick(PointerEventData eventData) {
        if (takenBy != -1)
            return;
        Debug.Log("Clicked on " + (Characters)transform.GetSiblingIndex());
        base.OnPointerClick(eventData);
        parent.RegisterCharacter(transform.GetSiblingIndex());
    }

}