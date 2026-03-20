using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;        // player object
    public Vector3 offset;          // distance between the camera and player, adjust in inspector
    public float followSpeed = 5f;  // how fast the camera follows the player

    void LateUpdate()
    {
        // desired position
        Vector3 targetPosition = player.position + offset;

        // smooth interpolation
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}