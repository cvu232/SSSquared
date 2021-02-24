using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Rudimentary Block functions. Attach this script to a Block.
 */

public class BlockBase : MonoBehaviour
{
    private GameObject blockEffect;
    public bool buildReady; // is build state
    public bool inBadSpace; // is in invalid build space
    public bool isPlaced;

    public MeshRenderer meshRenderer;

    public Material material;
    public Material holo;

    public bool triggered;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        inBadSpace = false;
        buildReady = true;
        isPlaced = false;
        try
        {
            GetBlockEffect();
        }
        catch { }
        if (blockEffect)
            blockEffect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // destroy when falling through
        if (other.CompareTag("doom"))
            Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        triggered = true;
        // check if too close to ground/other blocks
        if (other.CompareTag("block") || other.CompareTag("bounds") && !isPlaced)
        {
            inBadSpace = true;
            meshRenderer.material = holo;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        triggered = false;
        // check if not intruding ground/other blocks
        if (other.CompareTag("block") || other.CompareTag("bounds") && !isPlaced)
        {
            inBadSpace = false;
            meshRenderer.material = material;
        }
    }

    public void place()
    {
        buildReady = false;
        isPlaced = true;
        if (blockEffect)
            blockEffect.SetActive(true);
    }

    public void GetBlockEffect()
    {
        if (transform.GetChild(0).gameObject)
            blockEffect = transform.GetChild(0).gameObject;
        else blockEffect = null;
    }

    // delete the game object
    public void delete()
    {
        Destroy(gameObject);
    }


}
