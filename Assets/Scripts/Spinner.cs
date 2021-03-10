﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Spinner : MonoBehaviour {

    public Vector3 rot;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        transform.Rotate(rot * Time.deltaTime);

    }

}