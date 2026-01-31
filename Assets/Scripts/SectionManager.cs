using UnityEngine;
using System.Collections.Generic;

// this script really needs some work its so jank sometimes when on play i will kms
// prefab spawning isnt perfect, this probably will need an overhaul but hey, its a prototype
// lets avoid making each script highly dependent on each other pls

public class SectionManager : MonoBehaviour
{
    [Header("Level Settings")]
    // I LOVE LISTS
    public GameObject[] sectionPrefabs;     // list of section prefabs to spawn, drag in inspector

    public float sectionLength = 15f;       // length of each section (very jank, seriously, we need a way to make this work consistently)
    public int maxSections = 6;             // max number of active sections 

    private float nextSpawnZ = 0f;          // z position to spawn next section (jank)

    // a list of ALL ACTIVE SECTIONS during play
    private List<GameObject> activeSections = new List<GameObject>();

    void Start()
    {
        // spawn initial sections
        for (int i = 0; i < maxSections; i++)
        {
            SpawnSection();
        }
    }

    // idk how to set up random object placement within a prefab
    // probably different game objects scattered in a prefab to act as spawnpoints for different objects

    void SpawnSection()
    {
        // pick a random prefab from the list
        GameObject prefab = sectionPrefabs[Random.Range(0, sectionPrefabs.Length)];

        // instantiate it
        GameObject section = Instantiate(prefab, new Vector3(0, 0, nextSpawnZ), Quaternion.identity);

        activeSections.Add(section);

        // update the next spawn pos
        nextSpawnZ += sectionLength;
    }

    public void RemoveSection(GameObject section)
    {
        if (activeSections.Contains(section))
        {
            activeSections.Remove(section);
            Destroy(section);

            // spawn a new section immediately to keep level going
            SpawnSection();

            //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH
        }
    }
}
