using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public bool IsGameRunning = true;       // for game manager handling
    public Transform[] spawnPoints;         // 9 spawn points
    public GameObject[] prefabs;            // building prefabs
    public float spawnInterval;             // time between spawns
    public float destroyDelay;              // lifetime of spawned objects

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (IsGameRunning)
        {
            SpawnPrefabs();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPrefabs()
    {
        foreach (Transform point in spawnPoints)
        {
            //pick a random index in the array
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

            // instantiate based on randomly chosen array index
            GameObject obj = Instantiate(prefab, point.position, point.rotation);

            // destroy after set duration
            Destroy(obj, destroyDelay);
        }
    }
}