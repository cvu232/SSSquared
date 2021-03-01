using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * Object Spawner. Spawns objects like projectiles.
 */
public class ObjectSpawner : MonoBehaviour
{
    public bool spawning;
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
        yield return new WaitForSeconds(5);
        spawning = false;
    }
}
