using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;            // player object
    public Vector3 offset;              // distance between the camera and player, adjust in inspector
    public float baseFollowSpeed = 7f;  // base speed for camera follow

    private GameManager gameManager;
    private PlayerMove _player;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        _player = FindAnyObjectByType<PlayerMove>();
    }

    void LateUpdate()
    {
        // camera is not parented to the player, so this stops it from acting on its own when the game is paused
        if (gameManager.currentState != GameStates.RUNNING) return;

        // dynamic follow speed based on player's forward speed
        // subtle scaling where camera becomes faster as player goes faster
        // avoids having the camera lag too far when speed is capped
        float speedFactor = gameManager.currentSpeed / _player.maxForwardSpeed;
        float dynamicFollowSpeed = baseFollowSpeed * (1f + speedFactor * 0.5f);

        // desired position
        Vector3 targetPosition = player.position + offset;

        // smooth interpolation
        transform.position = Vector3.Lerp(transform.position, targetPosition, dynamicFollowSpeed * Time.deltaTime);
    }
}