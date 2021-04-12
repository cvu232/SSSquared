using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayingSpacemen : MonoBehaviour {

    public static bool showCredits;
    protected bool isCredits;

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

    protected SpriteRenderer renderer;
    private float offset;

    // Start is called before the first frame update
    protected void Start() {

        globalMulti = Random.Range(globalMultiMin, globalMultiMax);
        orbitRadius = Random.Range(orbitRadiusMin, orbitRadiusMax);
        rotSpeed = Random.Range(rotSpeedMin, rotSpeedMax) * (Random.Range (0, 2) == 0 ? 1 : -1);
        offset = Random.Range(0, 1/orbitSpeed);

        transform.Translate(Vector3.forward * orbitRadius);

        renderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    protected void Update() {

        transform.localPosition = new Vector3(
            Mathf.Sin(Time.time * orbitSpeed * globalMulti + offset) * orbitRadius,
            Mathf.Cos(Time.time * orbitSpeed * globalMulti + offset) * orbitRadius,
            transform.localPosition.z
        );

        transform.rotation = Quaternion.Euler(0, 0, Time.time * rotSpeed * globalMulti + offset);

        Color col = renderer.color;
        col.a = Application.isPlaying ? Mathf.Lerp(col.a, showCredits == isCredits ? 1 : 0, Time.deltaTime * 0.5f) : 1;
        renderer.color = col;

    }

}