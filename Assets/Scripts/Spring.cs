using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            rb.velocity = new Vector2(rb.velocity.x, 0); // reset y velo
            rb.AddForce(Vector2.up * 75, ForceMode2D.Impulse);
        }
    }
}
