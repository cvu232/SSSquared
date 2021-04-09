using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * Object Spawner. Spawns objects like blocks or projectiles.
 */
public class ObjectSpawner : MonoBehaviour
{
    public bool spawning;
    public int spawnInterval = 5;
    public GameObject spawnedObj;
    public GameObject spawnedObjAlt;
    [Range(0, 1)]
    public float spawnedObjAltLikelyhood;
    private GameObject workingObj;
    private Collider2D[] coll;
    private BlockBase block;

    private void Start()
    {
        spawning = false;
        coll = GetComponentsInParent<Collider2D>();
        block = GetComponentInParent<BlockBase>();
    }

    private void Update()
    {
        if (!spawning)
        {
            StartCoroutine(SpawnObject());
        }
    }

    IEnumerator SpawnObject()
    {
        spawning = true;
        workingObj = Instantiate(Random.Range(0, 1f) < spawnedObjAltLikelyhood ? spawnedObjAlt : spawnedObj, transform.position, Quaternion.identity);
        workingObj.GetComponent<Projectile>().level = block.level;
        foreach (Collider2D coll in coll)
            Physics2D.IgnoreCollision(coll, workingObj.GetComponent<Collider2D>());
        //Rigidbody2D rb = workingObj.GetComponent<Rigidbody2D>();
        yield return new WaitForSeconds(spawnInterval);
        spawning = false;
    }
}
