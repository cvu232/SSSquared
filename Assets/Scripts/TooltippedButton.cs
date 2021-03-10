using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltippedButton : EventTrigger {

    [SerializeField]
    public string tooltipInfo;

    public override void OnPointerEnter(PointerEventData data) {
        TooltipUI.instance.currentButton = this;
    }

    public override void OnPointerExit(PointerEventData data) {
        if (TooltipUI.instance.currentButton == this) {
            TooltipUI.instance.group.alpha = 0;
            TooltipUI.instance.currentButton = null;
        }
    }

}