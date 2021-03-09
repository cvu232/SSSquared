using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Level level;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (transform.position.y < level.transform.position.y + level.killHeight)
            Destroy(gameObject);

    }

}