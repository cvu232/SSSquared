using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Basic Block functions. Attach this script to a Block.
 */

public class BuildableObject : MonoBehaviour
{
    public ObjectEffect effect;
    public bool buildReady; // is build state
    public bool inBadSpace; // is in invalid build space
    public bool isPlaced;

    public MeshRenderer meshRenderer;
    public Material originalMaterial; // Set in Inspector the default material of the block
    public Material transparentRedMaterial; // Material to indicate encroachment with other block
    public Vector3 pos;

    public AudioClip createBlockSFX;
    public AudioClip destroyBlockSFX;

    private int inColl;
    public Level level { get; private set; }

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        inBadSpace = false;
        buildReady = true;
        isPlaced = false; // additional checks
        InitializeEffect();
    }

    public void Update() {
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 25);
        inBadSpace = inColl != 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if too close to ground/other blocks
        if (collision.CompareTag("Hazard") || collision.CompareTag("block") || collision.CompareTag("bounds") && !isPlaced)
        {
            inColl++;
            meshRenderer.material = transparentRedMaterial;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // check if not intruding ground/other blocks
        if (collision.CompareTag("Hazard") || collision.CompareTag("block") || collision.CompareTag("bounds") && !isPlaced)
        {
            inColl--;
            meshRenderer.material = originalMaterial;
        }
    }

    public void place(Level level) // Place the block in the Level
    {
        AudioManager.instance.PlayClipAt(createBlockSFX, transform.position);
        buildReady = false;
        isPlaced = true;
        this.level = level;
        transform.parent = level.gameObject.transform; // Parent this object to the Level it's set in
        EnableEffect();
    }

    private void InitializeEffect()
    {
        // Get the GameObject with the Block's Effect
        if (effect = transform.GetComponentInChildren<ObjectEffect>()) // Should be in the first child
            effect.enabled = false;
    }
    
    public void EnableEffect()
    {
        if (effect)
            effect.enabled = true;
    }

    // Destroy the Block
    public void delete()
    {
        AudioManager.instance.PlayClipAt(destroyBlockSFX, transform.position);
        Destroy(gameObject);
    }


}
