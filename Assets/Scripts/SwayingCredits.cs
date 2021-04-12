using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SwayingCredits : SwayingSpacemen {

    public Sprite spiteNormal;
    public Sprite spiteMeme;

    [Range(0, 1)]
    public float memeLikelyhood;
    public float memeFrameDuration;

    private float memeFrameCounter;
    private CanvasGroup group;

    // Start is called before the first frame update
    void Start() {

        base.Start();

        Color col = renderer.color;
        col.a = 0;
        renderer.color = col;

        isCredits = true;

        group = GetComponentInChildren<CanvasGroup>();

    }

    // Update is called once per frame
    void Update() {

        if (Application.isPlaying)
            base.Update();

        memeFrameCounter += Time.deltaTime;

        if (memeFrameCounter >= memeFrameDuration) {
            memeFrameCounter = 0;
            renderer.sprite = Random.Range(0, 1f) < memeLikelyhood ? spiteMeme : spiteNormal;
        }

        group.transform.rotation = Quaternion.identity;
        group.alpha = renderer.color.a;

    }

}