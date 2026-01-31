using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Horizontal Clamp (X)")]
    public float maxXDistance = 4f;

    [Header("Vertical Clamp (Y)")]
    public float minY = -0.5f;
    public float maxY = 3f;

    private float startX;
    private float fixedZ;

    void Start()
    {
        startX = transform.position.x;   // horizontal center
        fixedZ = transform.position.z;   // lock Z forever, no going forward
    }

    void Update()
    {
        // get input from WASD
        // screw the new input system, we using the old one
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // set position based on GetAxis input
        transform.position += new Vector3(moveX * moveSpeed * Time.deltaTime, moveY * moveSpeed * Time.deltaTime, 0f);

        ClampPosition();
    }

    // prevents plane from overshooting or leaving the camera
    void ClampPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, startX - maxXDistance, startX + maxXDistance);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, fixedZ);
    }

    // future dev should make the plane tilt depending on which direction ehe
}
