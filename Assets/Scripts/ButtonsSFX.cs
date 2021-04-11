using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonsSFX : MonoBehaviour, IPointerEnterHandler
{
    // Set these in Inspector
    public AudioClip buttonHoverSFX;
    public AudioClip buttonClickSFX;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlayClipAt(buttonHoverSFX, transform.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.PlayClipAt(buttonClickSFX, transform.position);
    }
}
