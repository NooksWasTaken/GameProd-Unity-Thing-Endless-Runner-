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

    [Header("Dynamic Spawn Settings")]
    [SerializeField] private float baseSpawnInterval = 5f; // base interval for scaling

    private GameManager gameManager;
    private float spawnTimer;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        spawnTimer = baseSpawnInterval;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // only spawn when the game is running
            if (gameManager.currentState == GameStates.RUNNING)
            {
                // dynamically adjust interval based on forward speed
                float speedFactor = gameManager.currentSpeed / 20000f; // subtle effect
                float adjustedInterval = Mathf.Max(0.1f, baseSpawnInterval * (1f - speedFactor));

                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0f)
                {
                    SpawnBuildingPrefabs();
                    SpawnPowerupPrefabs();
                    spawnTimer = adjustedInterval;
                }
            }

            yield return null;
        }
    }

    void SpawnBuildingPrefabs()
    {
        foreach (Transform point in buildingSpawnPoints)
        {
            GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];

            // spawn 5 units below
            Vector3 spawnPos = point.position + Vector3.down * 40f;

            GameObject obj = Instantiate(prefab, spawnPos, point.rotation);

            // start animation with slight random delay
            StartCoroutine(AnimateSpawn(obj.transform, point.position));

            Destroy(obj, destroyDelay);
        }
    }

    private IEnumerator AnimateSpawn(Transform obj, Vector3 targetPos)
    {
        float delay = Random.Range(0f, 0.5f); // desync, no animation in unison
        yield return new WaitForSeconds(delay);

        float duration = 0.2f; // how fast it rises (lower = faster)
        float time = 0f;

        Vector3 startPos = obj.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // smooth lerp (ease out)
            t = Mathf.SmoothStep(0f, 1f, t);

            obj.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        obj.position = targetPos; // snap to final just in case
    }

    void SpawnPowerupPrefabs()
    {
        Debug.Log("POWER SPAWN TEST");
        float chanceToSpawn = Random.value;
        if (chanceToSpawn <= PowerupSpawnChance)
        {
            foreach (Transform point in powerSpawnPoints)
            {
                GameObject prefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
                GameObject obj = Instantiate(prefab, point.position, point.rotation);
                Destroy(obj, destroyDelay);
            }
        }
    }
}