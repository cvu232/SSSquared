using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour {

    public bool show;
    public float lerpSpeed;

    private RectTransform transform;
    private CanvasGroup group;

    // Start is called before the first frame update
    void Start() {

        transform = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();

    }

    // Update is called once per frame
    void Update() {

        transform.anchoredPosition = Vector3.Lerp (transform.anchoredPosition, Vector3.down * (show ? 0 : 500), Time.deltaTime * lerpSpeed);

        group.alpha = Mathf.Lerp(group.alpha, show ? 1 : 0, Time.deltaTime * lerpSpeed);
        group.interactable = show;
        group.blocksRaycasts = show;

    }

}