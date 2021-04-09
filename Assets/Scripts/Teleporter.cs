using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public BlockBase block;

    public Teleporter pair;
    public bool isPaired;

    public bool portReady;

    public Material portalMat; // Set in inspector
    public Material colouredMat;
    public Material voidMat; // Set in inspector
    public Color colour;

    private void Start()
    {
        // Get the Block this Teleporter is attached to
        block = transform.parent.gameObject.GetComponent<BlockBase>();

        pair = null;
        isPaired = false;

        portReady = true; // can use portal

        StartCoroutine(SetupOnPlacement());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If player enters the trigger and the Teleporter has a pair
        // Teleport the player to the paired Teleporter
        if (other.GetComponent<Player>() && pair && isPaired && portReady)
        {
            other.transform.position = new Vector2(pair.transform.position.x, pair.transform.position.y); // Move player to connected Portal
            StartCoroutine(DisablePairFor(1.0f)); // Disable portal pair for x time

            //if (Input.GetButtonDown(player.movement.inputVerticalAxisName)) // If Player presses their Jump key while touching Teleporter trigger
        }
    }

    private void searchForTeleporterPair()
    {
        // Get list of Teleporters in the level
        Teleporter[] candidates = block.level.gameObject.GetComponentsInChildren<Teleporter>();

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
                    other.StartCoroutine(UnpairIfMissing());

                    ColourPortals(); // Give portals colour to indicate which are connected

                    break;
                }
            }
        }
    }

    private void ColourPortals()
    {
        // Create a new colour if n/a to distinguish paired Teleporters
        if (!colouredMat)
        {
            // Generate random colour
            colour = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            colouredMat = new Material(portalMat); // New Material
            colouredMat.color = colour; // Set Material Colour
            pair.colouredMat = colouredMat; // Give this coloured Material to pair too

            // Set the new colour
            Renderer thisPortal = GetComponent<Renderer>();
            Renderer otherPortal = pair.GetComponent<Renderer>();
            thisPortal.material = colouredMat;
            otherPortal.material = colouredMat;
        }
    }

    IEnumerator DisablePairFor(float n)
    {
        portReady = false;
        pair.portReady = false;

        // disabled portal visual
        GetComponent<Renderer>().material = voidMat;
        pair.GetComponent<Renderer>().material = voidMat;

        yield return new WaitForSeconds(n);

        portReady = true;
        pair.portReady = true;

        GetComponent<Renderer>().material = colouredMat;
        pair.GetComponent<Renderer>().material = colouredMat;
    }

    IEnumerator SetupOnPlacement()
    {
        // Wait until placed before setting the teleporter
        yield return new WaitUntil(()=>block.isPlaced);
        searchForTeleporterPair();
    }

    IEnumerator UnpairIfMissing()
    {
        // When pair is disconnected
        yield return new WaitUntil(() => !pair);
        pair = null;
        isPaired = false;
        GetComponent<Renderer>().material = voidMat;
        // Search for new pair
        searchForTeleporterPair();
    }
}
