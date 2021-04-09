using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayingSpacemen : MonoBehaviour {

    public float globalMultiMin = 1;
    public float globalMultiMax = 2;
    public float orbitRadiusMin = 1;
    public float orbitRadiusMax = 2;
    public float rotSpeedMin = 1;
    public float rotSpeedMax = 2;
    public float orbitSpeed = 1 ;

    private float globalMulti;
    private float orbitRadius;
    private float rotSpeed;

    private float offset;

    // Start is called before the first frame update
    void Start() {

        globalMulti = Random.Range(globalMultiMin, globalMultiMax);
        orbitRadius = Random.Range(orbitRadiusMin, orbitRadiusMax);
        rotSpeed = Random.Range(rotSpeedMin, rotSpeedMax) * (Random.Range (0, 2) == 0 ? 1 : -1);
        offset = Random.Range(0, 1/orbitSpeed);

        transform.Translate(Vector3.forward * Random.Range(orbitRadiusMin, orbitRadiusMax));

    }

    // Update is called once per frame
    void Update() {

        transform.position = new Vector3(
            Mathf.Sin(Time.time * orbitSpeed * globalMulti + offset) * orbitRadius,
            Mathf.Cos(Time.time * orbitSpeed * globalMulti + offset) * orbitRadius,
            transform.position.z
        );

        transform.rotation = Quaternion.Euler(0, 0, Time.time * rotSpeed * globalMulti + offset);

    }

}