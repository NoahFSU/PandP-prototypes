using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;

    [SerializeField] Transform[] spawnPos;

    [SerializeField] int numberToSpawn;
    [SerializeField] int spawnTimer;

    int spawnCount;

    bool isSpawning;
    bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager.Instance.updateGameGoal(numberToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if(startSpawning && !isSpawning && spawnCount < numberToSpawn)
        {
            StartCoroutine(Spawn());    
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;

        int arrayPos = Random.Range(0, spawnPos.Length);
        Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        spawnCount++;

        yield return new WaitForSeconds(spawnTimer);

        isSpawning = false;
    }
}
