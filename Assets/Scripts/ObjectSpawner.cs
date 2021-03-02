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
    private GameObject workingObj;

    private void Start()
    {
        spawning = false;
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
        workingObj = Instantiate(spawnedObj, transform.position, Quaternion.identity);
        //Rigidbody2D rb = workingObj.GetComponent<Rigidbody2D>();
        yield return new WaitForSeconds(spawnInterval);
        spawning = false;
    }
}
