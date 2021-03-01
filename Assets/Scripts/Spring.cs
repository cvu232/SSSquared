using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.CompareTag("Player"))
        {
            other.attachedRigidbody.AddRelativeForce(Vector3.up * 100, ForceMode2D.Impulse);
        }
    }
}
