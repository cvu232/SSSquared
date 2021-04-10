using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{

    private static int index;

    public BlockBase block;

    public Teleporter pair;
    public bool isPaired;

    public bool portReady;

    public Color colour;

    public ParticleSystem portalParticleSystem;
    public ParticleSystem portalInwardParticleSystem;

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

                    index++;

                    ColourPortals(); // Give portals colour to indicate which are connected

                    break;
                }
            }
        }
    }

    private void ColourPortals()
    {
        // Create a new colour if n/a to distinguish paired Teleporters
        if (colour != null)
        {
            // Generate non-random colour
            colour = Color.HSVToRGB(index * 0.2f, 1, 1);
            Debug.Log (Color.HSVToRGB(index * 0.2f, 1, 1));

            ColourPortalParticleSystems();
        }
    }

    private void ColourPortalParticleSystems() //method for coloring the portals
    {
        ParticleSystem.MainModule portalParticleSystemMain = portalParticleSystem.main;
        ParticleSystem.MainModule portalInwardParticleSystemMain = portalInwardParticleSystem.main;

        portalParticleSystemMain.startColor = colour;
        portalInwardParticleSystemMain.startColor = colour;

        ParticleSystem.MainModule portalParticleSystemMain2 = pair.portalParticleSystem.main;
        ParticleSystem.MainModule portalInwardParticleSystemMain2 = pair.portalInwardParticleSystem.main;

        portalParticleSystemMain2.startColor = colour;
        portalInwardParticleSystemMain2.startColor = colour;


    }

    IEnumerator DisablePairFor(float n)
    {
        portReady = false;
        pair.portReady = false;

        yield return new WaitForSeconds(n);

        portReady = true;
        pair.portReady = true;
    }

    IEnumerator SetupOnPlacement()
    {
        // Wait until placed before setting the teleporter
        yield return new WaitUntil(()=>block.isPlaced);

        // If player is trying to exceed 6 pairs of Teleporters
        if (index > 5)
        {
            block.delete();
        }

        searchForTeleporterPair();
    }

    IEnumerator UnpairIfMissing()
    {
        // When pair is disconnected
        yield return new WaitUntil(() => !pair);
        pair = null;
        isPaired = false;
        index--;
        // Search for new pair
        searchForTeleporterPair();
    }
}
