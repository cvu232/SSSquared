using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * Block Spawner. Spawns blocks that drop down.
 */
public class BlockSpawner : MonoBehaviour
{
    public BlockBase spawnedBlock;
    private BlockBase workingBlock;

    public bool spawning;

    private void Start()
    {
        spawning = false;
    }

    private void Update()
    {
        if (!spawning)
        {
            StartCoroutine(SpawnBlock());
        }
    }

    IEnumerator SpawnBlock()
    {
        spawning = true;
        workingBlock = Instantiate(spawnedBlock, transform.position, Quaternion.identity);
        // drop block
        workingBlock.transform.localScale = new Vector3(3f, 3f, 3f); // fix block size
        workingBlock.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        workingBlock.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        workingBlock.gameObject.layer = LayerMask.NameToLayer("Blocks"); // allows block to fall through other block
        yield return new WaitForSeconds(5);
        spawning = false;
    }
}
