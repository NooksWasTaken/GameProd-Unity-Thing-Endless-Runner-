using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    GameManager gameManager;
    GameStates gameStates;

    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Horizontal Clamp (X)")]
    public float maxXDistance = 4f;

    [Header("Vertical Clamp (Y)")]
    public float minY = -0.5f;
    public float maxY = 3f;

    private float startX;
    private float fixedZ;

    private float originalSpeed;
    private bool isinvincible;
    private Coroutine speedCoroutine;
    private Coroutine invCoroutine;

    internal Vector3 InitialPosition;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        startX = transform.position.x;   // horizontal center
        fixedZ = transform.position.z;   // lock Z forever, no going forward
        isinvincible = false;
        InitialPosition = transform.position;
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


    // Checks for collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle") && !isinvincible)
        {
            gameManager.Lives--;
            //GameAnalyticsManager.instance.FunnelFinished(3, "Player_Obtained_Powerup");
        }
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    public void ActivateInvincibility(float duration)
    {
        if (invCoroutine != null)
            StopCoroutine(invCoroutine);

        invCoroutine = StartCoroutine(InvincibilityRoutine(duration));
    }

    IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        Debug.Log("Speed boost ON");

        originalSpeed = moveSpeed;
        moveSpeed = originalSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;

        Debug.Log("Speed boost OFF");
    }

    IEnumerator InvincibilityRoutine(float duration)
    {
        Debug.Log("Invincibility ON");
        isinvincible = true;

        yield return new WaitForSeconds(duration);
        isinvincible = false;

        Debug.Log("Invincibility OFF");
    }

}
