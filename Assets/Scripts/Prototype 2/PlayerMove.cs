using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    GameManager gameManager;
    public Rigidbody rb;

    [Header("Player Forces")]     // player control strength
    public float forwardForce;
    public float upForce;
    public float sideForce;

    [Header("Tilt Settings")]
    public float tiltAngle = 15f; // maximum tilt in degrees
    public float tiltSpeed = 5f;  // how fast the tilt interpolates

    private float originalSpeed;
    private Coroutine speedCoroutine;

    public Vector3 InitialPosition;

    [HideInInspector] public bool canMove = false; // controlled by GameManager

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();

        InitialPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!canMove) return;

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

    private void OnCollisionEnter(Collision collision)
    {
        // reduce lives on obstacle collision
        if (collision.collider.CompareTag("Obstacle"))
        {
            gameManager.Lives--;

            if (gameManager.Lives <= 0)
                gameManager.SetGameState(GameStates.GAMEOVER);
        }
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    public IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        Debug.Log("Speed boost ON");

        this.gameObject.tag = "Untagged";   // bandaid solution, currently stacking speed buffs makes the buffed speed its new permanent speed
                                            // so we just change the player tag so colliding with the power up doesnt trigger the buff
        originalSpeed = forwardForce;
        forwardForce = originalSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        forwardForce = originalSpeed;
        this.gameObject.tag = "Player";     // set it back to player tag

        Debug.Log("Speed boost OFF");
    }
}