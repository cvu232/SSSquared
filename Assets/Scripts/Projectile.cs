using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Level level;


    void Update() {
        if (transform.position.y < level.transform.position.y + level.killHeight)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() || collision.CompareTag("block"))
        {
            if (collision.GetComponent<Player>())
                collision.GetComponent<Player>().movement.Die();
            Destroy(gameObject);
        }
    }

}