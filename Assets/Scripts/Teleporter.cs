using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : ObjectEffect
{
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

    public List<ParticleSystem> portalParticleSystems = new List<ParticleSystem>();

    private void Start()
    {
        // Get the Block this Teleporter is attached to
        block = transform.parent.gameObject.GetComponent<BuildableObject>();

        pair = null;
        isPaired = false;

        portReady = true; // can use portal

        audioSource = GetComponent<AudioSource>();

        portalParticleSystems.Add(portalParticleSystem); //The portal particle system list does not include the burst on entry particle system
        portalParticleSystems.Add(portalInwardParticleSystem);
        portalParticleSystems.Add(portalBigCenterParticleSystem);
        portalParticleSystems.Add(portalExtraOutwardParticleSystem);

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

                    block.level.portalPairs++;

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
        colour = Color.HSVToRGB(block.level.portalPairs * 0.2f, 1, 1);
        pair.colour = colour;
        Debug.Log(Color.HSVToRGB(block.level.portalPairs * 0.2f, 1, 1));
        ColourPortalParticleSystems();
    }

    private void ColourPortalParticleSystems() //method for coloring the portals
    {
        
        for (int i = 0; i < portalParticleSystems.Count; i++)
        {
            ParticleSystem.MainModule particleSystemMain = portalParticleSystems[i].main;
            particleSystemMain.startColor = colour;

            ParticleSystem.MainModule particleSystemMain2 = pair.portalParticleSystems[i].main;
            particleSystemMain2.startColor = colour;
        }

        ParticleSystem.MainModule portalOutwardParticleSystemMain = portalOutwardParticleSystem.main;
        portalOutwardParticleSystemMain.startColor = colour;

        ParticleSystem.MainModule portalOutwardParticleSystemMain2 = pair.portalOutwardParticleSystem.main;
        portalOutwardParticleSystemMain2.startColor = colour;

    }

    private void TogglePortalParticleSystem()
    {
        if (portReady == false || pair.portReady == false)
        {
            for (int i = 0; i < portalParticleSystems.Count; i++)
            {
                ParticleSystem.MainModule particleSystemMain = portalParticleSystems[i].main;
                particleSystemMain.loop = false;

                ParticleSystem.MainModule particleSystemMain2 = pair.portalParticleSystems[i].main;
                particleSystemMain2.loop = false;
            }

            portalOutwardParticleSystem.Play();
            pair.portalOutwardParticleSystem.Play();

        }

        Invoke(nameof(TurnPortalParticleSystemBackOn), 0.5f);

    }

    private void TurnPortalParticleSystemBackOn()
    {

        for (int i = 0; i < portalParticleSystems.Count; i++)
        {
            ParticleSystem.MainModule particleSystemMain = portalParticleSystems[i].main;
            particleSystemMain.loop = true;

            ParticleSystem.MainModule particleSystemMain2 = pair.portalParticleSystems[i].main;
            particleSystemMain2.loop = true;
        }

        for (int i = 0; i < portalParticleSystems.Count; i++)
        {
            portalParticleSystems[i].Play();
            pair.portalParticleSystems[i].Play();
        }

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
        if (block.level.portalPairs > block.level.portalPairLimit) // delete block if exceeding max pairs of teleporters in level
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
        block.level.portalPairs--;
        block.delete();
    }
}
