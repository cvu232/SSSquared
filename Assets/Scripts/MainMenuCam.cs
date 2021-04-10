using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera))]
public class MainMenuCam : MonoBehaviour {

    public float speed;

    private float finalFOV;

    private Camera cam;

    // Start is called before the first frame update
    void Start() {

        cam = GetComponent<Camera>();

        finalFOV = cam.fieldOfView;

        cam.fieldOfView = 1;

    }

    // Update is called once per frame
    void Update() {

        if (cam.fieldOfView >= finalFOV)
            speed = Mathf.Lerp(speed, 0, Time.deltaTime * 0.1f);

        cam.fieldOfView = cam.fieldOfView + (speed * Time.deltaTime);

    }

}