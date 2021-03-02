using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Basic Block functions. Attach this script to a Block.
 */

public class BlockBase : MonoBehaviour
{
    public GameObject blockEffect;
    public bool buildReady; // is build state
    public bool inBadSpace; // is in invalid build space
    public bool isPlaced;

    public MeshRenderer meshRenderer;
    public Material material;
    public Material holo;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        inBadSpace = false;
        buildReady = true;
        isPlaced = false; // additional checks
        GetSetupBlockEffect();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // check if too close to ground/other blocks
        if (collision.CompareTag("block") || collision.CompareTag("bounds") && !isPlaced)
        {
            inBadSpace = true;
            meshRenderer.material = holo;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // check if not intruding ground/other blocks
        if (collision.CompareTag("block") || collision.CompareTag("bounds") && !isPlaced)
        {
            inBadSpace = false;
            meshRenderer.material = material;
        }
    }

    public void place()
    {
        buildReady = false;
        isPlaced = true;
        ActivateBlockEffect();
    }

    public void GetSetupBlockEffect()
    {
        try
        {
            if (transform.GetChild(0))
                if (transform.GetChild(0).gameObject)
                    blockEffect = transform.GetChild(0).gameObject;
                else
                    blockEffect = null;
        }
        catch { }
        if (blockEffect)
            blockEffect.SetActive(false);
    }
    
    public void ActivateBlockEffect()
    {
        if (blockEffect)
            blockEffect.SetActive(true);
    }

    // delete the game object
    public void delete()
    {
        Destroy(gameObject);
    }


}
