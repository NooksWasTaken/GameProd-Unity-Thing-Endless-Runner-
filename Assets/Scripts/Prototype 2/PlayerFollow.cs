using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;      // adjust offset if needed, default at 0

    private float fixedY;

    void Start()
    {
        // store the spawners original Y position
        fixedY = transform.position.y;
    }

    // we don't want the spawner to spawn buildings at varying elevations
    // so we fix the Y position to make it all consistent
    // the building prefabs themselves should vary in height, not the spawn point
    
    void Update()
    {
        Vector3 newPosition = player.position + offset;

        // lock Y axis
        newPosition.y = fixedY;

        transform.position = newPosition;
    }
}