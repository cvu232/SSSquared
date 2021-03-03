using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.GetComponent<Level>().winner = collision.gameObject.GetComponent<Player>();
            GamePhaseManager.instance.currentPhaseTimer = 0;
        }
    }
}
