﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SideSpring : ObjectEffect
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            rb.velocity = new Vector2(rb.velocity.x, 0); // reset y velo
            int dir = Math.Sign(rb.position.x - transform.position.x);
            rb.AddForce((new Vector2(dir, 0) * 60) + (Vector2.up * 50), ForceMode2D.Impulse);
        }
    }
}