using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doomed : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("doom"))
        {
            Destroy(gameObject);
        }
    }
}
