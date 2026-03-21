using System.Collections;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Building Spawnpoints")]
    public Transform[] buildingSpawnPoints; // building spawnpoints

    [Header("Building Prefabs")]
    public GameObject[] buildingPrefabs;    // building prefabs

    [Header("Powerup Spawnpoints")]
    public Transform[] powerSpawnPoints;    // power up spawnpoints

    [Header("Powerup Prefabs")]
    public GameObject[] powerupPrefabs;     // building prefabs

    [Header("Spawner Settings")]
    [Range(0, 1)]
    public float PowerupSpawnChance;
    public float spawnInterval;             // time between spawns
    public float destroyDelay;              // lifetime of spawned objects

    private GameManager gameManager;
    private float spawnTimer;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        spawnTimer = spawnInterval;
        StartCoroutine(SpawnRoutine());
    }
    
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // only spawn when the game is paused
            if (gameManager.currentState == GameStates.RUNNING)
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0f)
                {
                    SpawnBuildingPrefabs();
                    SpawnPowerupPrefabs();
                    spawnTimer = spawnInterval;
                }
            }

            yield return null;
        }
    }

    // i was thinking of doing weighted spawns where we can select which building has priority in spawning but idk how
    // currently when the game is paused, the spawn timer is completely reset so there is a brief period where nothing is spawning because
    // the timer does not retain the original time before the game was paused
    void SpawnBuildingPrefabs()
    {
        foreach (Transform point in buildingSpawnPoints)
        {
            // pick a random index in the array
            GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];

            // instantiate based on randomly chosen array index
            GameObject obj = Instantiate(prefab, point.position, point.rotation);

            // destroy after set duration
            Destroy(obj, destroyDelay);
        }
    }

    void SpawnPowerupPrefabs()
    {
        Debug.Log("POWER SPAWN TEST");
        float chanceToSpawn = Random.value;
        {
            if (chanceToSpawn <= PowerupSpawnChance)
            {
                foreach (Transform point in powerSpawnPoints)
                {
                    // pick a random index in the array
                    GameObject prefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];

                    // instantiate based on randomly chosen array index
                    GameObject obj = Instantiate(prefab, point.position, point.rotation);

                    // destroy after set duration
                    Destroy(obj, destroyDelay);
                }
            }
            
        }
    }
}