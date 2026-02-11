using UnityEngine;

public class MoveSection : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;     // constant speed for all sections (probably have to decouple this to scale speed the longer the level goes)
    public float destroyZ = 60f; // z position at which section is destroyed

    private SectionManager spawner;

    void Start()
    {
        // find the SectionManager in the scene
        spawner = FindFirstObjectByType<SectionManager>();
    }

    void Update()
    {
<<<<<<< Updated upstream
        // move section backward consistently
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
=======
        if (gameManager.IsInGame) // executes if the player is not dead
        {
            // move section backward consistently
            transform.Translate(Vector3.back * gameManager.LevelSpeed * Time.deltaTime, Space.World);
>>>>>>> Stashed changes

        // check if section reached destroy point
        if (transform.position.z < destroyZ)
        {
            spawner.RemoveSection(gameObject);
        }
    }

    // mark smells
}
