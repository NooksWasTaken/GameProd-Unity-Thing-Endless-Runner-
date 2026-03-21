using System.Collections;
using UnityEngine;

public class MoveSection : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;     // constant speed for all sections (probably have to decouple this to scale speed the longer the level goes)
    public float destroyZ = 60f; // z position at which section is destroyed

    private SectionManager spawner;
    private GameManager gameManager;

    void Start()
    {
        // find the SectionManager in the scene
        spawner = FindFirstObjectByType<SectionManager>();

        // looks for gameManager in the scene
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        // if GameManager exists and the game is not running, do nothing (avoids crashing)
        //if (gameManager != null && !gameManager.IsInGame)
            return;


        // move section backward
        //transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        // check if section reached destroy point
        if (transform.position.z < destroyZ)
        {
            spawner.RemoveSection(gameObject);
        }
    }
    // mark smells
}
