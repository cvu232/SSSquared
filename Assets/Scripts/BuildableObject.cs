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
    public bool isEncroaching; // is in invalid build space
    public bool isPlaced;

    private GameObject highlightMesh;
    private MeshRenderer hlMeshRenderer;
    public Material clearMaterial; // Set in Inspector the default material of the block
    public Material transparentRedMaterial; // Set in Inspector. Material to indicate encroachment with other block
    public Material doomedMaterial; // Set in Inspector. Material to indicate selected block to destroy
    public Vector3 pos;

    public AudioClip createBlockSFX; // Set in Inspector

    private int inColl;
    public Level level { get; private set; }

    private void Awake()
    {
        highlightMesh = transform.Find("HighlightMesh").gameObject;
        hlMeshRenderer = highlightMesh.GetComponentInChildren<MeshRenderer>();
        clearMaterial = hlMeshRenderer.material;
        isPlaced = false; // additional checks
    }

    private void Start()
    {
        isEncroaching = false;
        buildReady = true;
        InitializeEffect();
    }

    public void Update() {
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 25);
        isEncroaching = inColl != 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if too close to ground/other blocks
        if ((collision.CompareTag("Hazard") || collision.CompareTag("block") || collision.CompareTag("bounds") || collision.CompareTag("portal")) && !isPlaced)
        {
            inColl++;
            HighlightEncroachment();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // check if not intruding ground/other blocks
        if ((collision.CompareTag("Hazard") || collision.CompareTag("block") || collision.CompareTag("bounds") || collision.CompareTag("portal")) && !isPlaced)
        {
            inColl--;
            HighlightOff();
        }
    }

    public void place(Level level) // Place the block in the Level
    {
        AudioManager.instance.PlayClipAt(createBlockSFX, transform.position);
        HighlightOff();
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
        Destroy(gameObject);
    }

    public void HighlightOff()
    {
        hlMeshRenderer.material = clearMaterial;
    }
    public void HighlightEncroachment()
    {
        hlMeshRenderer.material = transparentRedMaterial;
    }

    public void HighlightTarget()
    {
        hlMeshRenderer.material = doomedMaterial;
    }

}
