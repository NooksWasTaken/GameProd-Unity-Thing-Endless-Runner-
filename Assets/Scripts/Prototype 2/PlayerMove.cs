using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Player Forces")]     // player control strength
    public float forwardForce;
    public float upForce;
    public float sideForce;

    [Header("Tilt Settings")]
    public float tiltAngle = 15f; // maximum tilt in degrees
    public float tiltSpeed = 5f;  // how fast the tilt interpolates

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Movement();
        ApplyTilt();
    }

    private void Movement()
    {
        // constant forward movement
        rb.AddForce(Vector3.forward * forwardForce * Time.fixedDeltaTime, ForceMode.Force);

        // move right
        if (Input.GetKey("d"))
        {
            rb.AddForce(Vector3.right * sideForce * Time.fixedDeltaTime, ForceMode.Force);
        }

        // move left
        if (Input.GetKey("a"))
        {
            rb.AddForce(Vector3.left * sideForce * Time.fixedDeltaTime, ForceMode.Force);
        }

        // move up
        if (Input.GetKey("w"))
        {
            rb.AddForce(Vector3.up * upForce * Time.fixedDeltaTime, ForceMode.Force);
        }

        // move down
        if (Input.GetKey("s"))
        {
            rb.AddForce(Vector3.down * upForce * Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    private void ApplyTilt()
    {
        // get input
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey("d")) moveX = 1f;
        if (Input.GetKey("a")) moveX = -1f;
        if (Input.GetKey("w")) moveY = 1f;
        if (Input.GetKey("s")) moveY = -1f;

        // target rotation based on input
        Quaternion targetRotation = Quaternion.Euler(-moveY * tiltAngle, 0, -moveX * tiltAngle);

        // smoothly interpolate to target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * tiltSpeed);
    }
}