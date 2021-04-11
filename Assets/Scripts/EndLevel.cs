using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public Level thislevel;

    private void Start()
    {
        thislevel = GetComponentInParent<Level>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && thislevel)
        {
            thislevel.winner = collision.gameObject.GetComponent<Player>();
            GamePhaseManager.instance.currentPhaseTimer = 0;
        }
    }
}
