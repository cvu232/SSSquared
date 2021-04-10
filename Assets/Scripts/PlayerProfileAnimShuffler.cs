using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Animator))]
public class PlayerProfileAnimShuffler : MonoBehaviour {

    public bool hasVariant;

    public float idleSwitchMin;
    public float idleSwitchMax;
    public float variantMin;
    public float variantMax;

    private Animator anim;
    private float idleSwitchTimer;
    private float variantTimer;
    private bool inVariant;

    // Start is called before the first frame update
    void Start() {

        anim = GetComponent<Animator>();

        idleSwitchTimer = Random.Range(idleSwitchMin, idleSwitchMax);
        variantTimer = Random.Range(variantMin, variantMax);

    }

    // Update is called once per frame
    void Update() {

        idleSwitchTimer -= Time.deltaTime;
        if (!inVariant)
            variantTimer -= Time.deltaTime;
        else
            variantTimer = Random.Range(variantMin, variantMax);

        if (hasVariant) {
            if (idleSwitchTimer <= 0) {
                idleSwitchTimer = Random.Range(idleSwitchMin, idleSwitchMax);
                inVariant = !inVariant;
                anim.SetBool("IdleVariant", inVariant);
                anim.ResetTrigger("Variant1");
                anim.ResetTrigger("Variant2");
            }
        } else
            inVariant = false;

        if (variantTimer <= 0) {
            variantTimer = Random.Range(variantMin, variantMax);
            anim.SetTrigger("Variant" + (Random.Range(0, 2) == 0 ? "1" : "2"));
        }

    }

}