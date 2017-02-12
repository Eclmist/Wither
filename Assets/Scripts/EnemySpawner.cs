using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    //Bound Ranges
    public float minBoundX;
    public float maxBoundX;
    public float minBoundZ;
    public float maxBoundZ;

    //Spawn Rate per Second
    [SerializeField]
    public float spawnRate;

    //Spawn Point
    protected Vector3 spawnPoint;

    //Enemy Prefab
    public GameObject enemy;

    //Timer
    private float timePassed = 0;

    void Update()
    {
        if(Time.time - timePassed > spawnRate / (spawnRate * spawnRate))
        {
            SpawnEnemy(1);
            timePassed = Time.time;
        }
    }

	public void SpawnEnemy(int enemyCount)
    {
        for(int i = 0; i < enemyCount; i++)
        {
            spawnPoint = new Vector3(transform.position.x + Random.Range(minBoundX, maxBoundX),
              transform.position.y,
              transform.position.z + Random.Range(minBoundZ, maxBoundZ));

            Instantiate(enemy, spawnPoint, enemy.transform.rotation);
        }
    }
}
