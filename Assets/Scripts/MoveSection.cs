using UnityEngine;

public class MoveSection : MonoBehaviour
{
    [Header("Movement Settings")]
    //public float speed = 7f;     // constant speed for all sections (probably have to decouple this to scale speed the longer the level goes)
    public float destroyZ = 60f; // z position at which section is destroyed

    private GameManager gameManager;
    private SectionManager spawner;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        // find the SectionManager in the scene
        spawner = FindFirstObjectByType<SectionManager>();
    }

    void Update()
    {
        if (gameManager.Lives > 0) // executes if the player is not dead
        {
            // move section backward consistently
            transform.Translate(Vector3.back * gameManager.LevelSpeed * Time.deltaTime, Space.World);

            // check if section reached destroy point
            if (transform.position.z < destroyZ)
            {
                spawner.RemoveSection(gameObject);
            }
        }
    }

    // mark smells
}
