﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doom : MonoBehaviour
{
    public Transform spawnpoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // if there is no spawnpoint then crash
            collision.transform.position = spawnpoint.position;
        }

        if (collision.gameObject.CompareTag("projectile"))
        {
            Destroy(collision.gameObject);
        }
    }
}