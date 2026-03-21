using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;        // player object
    public Vector3 offset;          // distance between the camera and player, adjust in inspector
    public float followSpeed = 7f;  // how fast the camera follows the player

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void LateUpdate()
    {
        // camera is not parented to the player, so this stops it from acting on it own when the game is paused
        if (gameManager.currentState != GameStates.RUNNING) return;

        // desired position
        Vector3 targetPosition = player.position + offset;

        // smooth interpolation
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}