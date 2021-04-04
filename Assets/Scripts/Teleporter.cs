using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public BlockBase block;

    public Teleporter pair;
    public bool isPaired;
    
    private void Start()
    {
        // Get the Block this Teleporter is attached to
        block = transform.parent.gameObject.GetComponent<BlockBase>();

        pair = null;
        isPaired = false;
        StartCoroutine(SetupOnPlacement());
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // If player enters the trigger and the Teleporter has a pair
        // Teleport the player to the paired Teleporter
        if (other.GetComponent<Player>() && pair && isPaired)
        {
            Player player = other.GetComponent<Player>(); // Get Player

            if (Input.GetButtonDown(player.movement.inputVerticalAxisName)) // If Player presses their Jump key while touching Teleporter trigger
            {
                // Prevent player from jumping or add "interact" button

                // Teleport the Player to paired Teleporter
                other.transform.position = pair.transform.position;
            }
        }
    }

    private void searchForTeleporterPair()
    {
        Teleporter[] candidates;
        // Get list of Teleporters in the level
        candidates = block.level.gameObject.GetComponentsInChildren<Teleporter>();
        //candidates = FindObjectsOfType<Teleporter>();

        // If there are Teleporter objects
        if (candidates.Length > 0)
        {
            foreach (Teleporter other in candidates)
            {
                // If this candidate is not itself and not paired, pair these teleporters
                if (other != this && !other.isPaired)
                {
                    // Set this pair to other
                    pair = other;
                    isPaired = true; // Set this to paired
                    other.pair = this;
                    other.isPaired = true;
                    // If pair is deleted then this is not paired
                    StartCoroutine(UnpairIfMissing());

                    // Create a new colour to distinguish paired Teleporters
                    Material mat = new Material(Shader.Find("Specular"));
                    Color colour = Random.ColorHSV();
                    mat.color = colour;

                    // Set the new colour
                    Renderer thisPortal = GetComponent<Renderer>();
                    Renderer otherPortal = other.GetComponent<Renderer>();
                    thisPortal.material = mat;
                    otherPortal.material = mat;

                    break;
                }
            }
        }
    }

    IEnumerator SetupOnPlacement()
    {
        // Wait until placed before setting the teleporter
        yield return new WaitUntil(()=>block.isPlaced);
        searchForTeleporterPair();
    }

    IEnumerator UnpairIfMissing()
    {
        // If pair is deleted then this is not paired
        yield return new WaitUntil(() => !pair);
        pair = null;
        isPaired = false;
    }
}
