using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : ObjectEffect
{

    private static int index;

    public BuildableObject block;

    public Teleporter pair;
    public bool isPaired;

    public bool portReady;
    public Color colour;

    private AudioSource audioSource;
    public AudioClip portalSFX;

    public ParticleSystem portalParticleSystem;
    public ParticleSystem portalInwardParticleSystem;
    public ParticleSystem portalOutwardParticleSystem;
    public ParticleSystem portalBigCenterParticleSystem;
    public ParticleSystem portalExtraOutwardParticleSystem;

    private void Start()
    {
        // Get the Block this Teleporter is attached to
        block = transform.parent.gameObject.GetComponent<BuildableObject>();

        pair = null;
        isPaired = false;

        portReady = true; // can use portal

        audioSource = GetComponent<AudioSource>();

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
        // Get array of Teleporters in the level
        Teleporter[] candidates = block.level.gameObject.GetComponentsInChildren<Teleporter>();

        // If there are Teleporter objects
        if (candidates.Length > 0)
        {
            foreach (Teleporter other in candidates)
            {
                // If this candidate is not itself and not paired, pair these teleporters
                if (other != this && !other.isPaired && other.enabled)
                {
                    // Set this pair to other
                    pair = other;
                    isPaired = true; // Set this to paired
                    other.pair = this;
                    other.isPaired = true;
                    // If pair is deleted then this is not paired
                    StartCoroutine(DeleteIfLostPair());
                    other.SyncCoroutine();

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
        // Generate non-random colour
        colour = Color.HSVToRGB(index * 0.2f, 1, 1);
        pair.colour = colour;
        Debug.Log(Color.HSVToRGB(index * 0.2f, 1, 1));
        ColourPortalParticleSystems();
    }

    private void ColourPortalParticleSystems() //method for coloring the portals
    {
        ParticleSystem.MainModule portalParticleSystemMain = portalParticleSystem.main;
        ParticleSystem.MainModule portalInwardParticleSystemMain = portalInwardParticleSystem.main;
        ParticleSystem.MainModule portalOutwardParticleSystemMain = portalOutwardParticleSystem.main;
        ParticleSystem.MainModule portalBigCenterParticleSystemMain = portalBigCenterParticleSystem.main;
        ParticleSystem.MainModule portalExtraOutwardParticleSystemMain = portalExtraOutwardParticleSystem.main;

        portalParticleSystemMain.startColor = colour;
        portalInwardParticleSystemMain.startColor = colour;
        portalOutwardParticleSystemMain.startColor = colour;
        portalBigCenterParticleSystemMain.startColor = colour;
        portalExtraOutwardParticleSystemMain.startColor = colour;

        ParticleSystem.MainModule portalParticleSystemMain2 = pair.portalParticleSystem.main;
        ParticleSystem.MainModule portalInwardParticleSystemMain2 = pair.portalInwardParticleSystem.main;
        ParticleSystem.MainModule portalOutwardParticleSystemMain2 = pair.portalOutwardParticleSystem.main;
        ParticleSystem.MainModule portalBigCenterParticleSystemMain2 = pair.portalBigCenterParticleSystem.main;
        ParticleSystem.MainModule portalExtraOutwardParticleSystemMain2 = pair.portalExtraOutwardParticleSystem.main;

        portalParticleSystemMain2.startColor = colour;
        portalInwardParticleSystemMain2.startColor = colour;
        portalOutwardParticleSystemMain2.startColor = colour;
        portalBigCenterParticleSystemMain2.startColor = colour;
        portalExtraOutwardParticleSystemMain2.startColor = colour;
    }

    private void TogglePortalParticleSystem()
    {
        if (portReady == false || pair.portReady == false)
        {
            ParticleSystem.MainModule portalParticleSystemMain = portalParticleSystem.main;
            ParticleSystem.MainModule portalInwardParticleSystemMain = portalInwardParticleSystem.main;
            ParticleSystem.MainModule portalBigCenterParticleSystemMain = portalBigCenterParticleSystem.main;

            portalParticleSystemMain.loop = false;
            portalInwardParticleSystemMain.loop = false;
            portalBigCenterParticleSystemMain.loop = false;

            ParticleSystem.MainModule portalParticleSystemMain2 = pair.portalParticleSystem.main;
            ParticleSystem.MainModule portalInwardParticleSystemMain2 = pair.portalInwardParticleSystem.main;
            ParticleSystem.MainModule portalBigCenterParticleSystemMain2 = pair.portalBigCenterParticleSystem.main;

            portalParticleSystemMain2.loop = false;
            portalInwardParticleSystemMain2.loop = false;
            portalBigCenterParticleSystemMain2.loop = false;

            portalOutwardParticleSystem.Play();
            pair.portalOutwardParticleSystem.Play();
        }

        Invoke(nameof(TurnPortalParticleSystemBackOn), 0.5f);

    }

    private void TurnPortalParticleSystemBackOn()
    {
        ParticleSystem.MainModule portalParticleSystemMain = portalParticleSystem.main;
        ParticleSystem.MainModule portalInwardParticleSystemMain = portalInwardParticleSystem.main;
        ParticleSystem.MainModule portalBigCenterParticleSystemMain = portalBigCenterParticleSystem.main;

        portalParticleSystemMain.loop = true;
        portalInwardParticleSystemMain.loop = true;
        portalBigCenterParticleSystemMain.loop = true;

        ParticleSystem.MainModule portalParticleSystemMain2 = pair.portalParticleSystem.main;
        ParticleSystem.MainModule portalInwardParticleSystemMain2 = pair.portalInwardParticleSystem.main;
        ParticleSystem.MainModule portalBigCenterParticleSystemMain2 = pair.portalBigCenterParticleSystem.main;

        portalParticleSystemMain2.loop = true;
        portalInwardParticleSystemMain2.loop = true;
        portalBigCenterParticleSystemMain2.loop = true;

        portalParticleSystem.Play();
        portalInwardParticleSystem.Play();
        portalBigCenterParticleSystem.Play();
        pair.portalParticleSystem.Play();
        pair.portalInwardParticleSystem.Play();
        pair.portalBigCenterParticleSystem.Play();
    }

    public void SyncCoroutine()
    {
        StartCoroutine(DeleteIfLostPair());
    }

    IEnumerator DisablePairFor(float n)
    {
        audioSource.PlayOneShot(portalSFX); // play portal sfx
        portReady = false;
        pair.portReady = false;
        TogglePortalParticleSystem();

        yield return new WaitForSeconds(n);

        portReady = true;
        pair.portReady = true;
    }

    IEnumerator SetupOnPlacement()
    {
        // Wait until placed before setting the teleporter
        yield return new WaitUntil(()=>block.isPlaced);
        if (index > 4) // delete block if exceeding 6 pairs of teleporters
            block.delete(); // nuke this here
        else
            searchForTeleporterPair();
    }

    IEnumerator UnpairIfLostPair()
    {
        // When pair is disconnected
        yield return new WaitUntil(() => !pair);
        pair = null;
        isPaired = false;
        // Search for new pair
        searchForTeleporterPair();
    }

    IEnumerator DeleteIfLostPair()
    {
        yield return new WaitUntil(() => !pair);
        pair = null;
        isPaired = false;
        index--;
        block.delete();
    }
}
