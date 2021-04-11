using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndLevel : MonoBehaviour
{
    public Level thislevel;
    private AudioSource audioSource;

    private void Start()
    {
        thislevel = GetComponentInParent<Level>();
        audioSource = GetComponent<AudioSource>(); // Set Audio clip in Inspector
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && thislevel)
        {
            thislevel.winner = collision.gameObject.GetComponent<Player>();
            GamePhaseManager.instance.currentPhaseTimer = 0;
            audioSource.Play();
        }
    }
}
