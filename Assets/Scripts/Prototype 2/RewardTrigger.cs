using System.Collections;
using UnityEngine;

public class RewardTrigger : MonoBehaviour
{
    GameManager gameManager;

    public int AdditionalPoints;
    int RandomMovementPath;
    public float MovementDist;
    public float Speed;

    Vector3 InitPos;


    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        InitPos = transform.position;
        RandomMovementPath = Random.Range(0, 3);
    }

    void Update()
    {
        // Randomized Movement
        // if RandomMovementPath is 0, object remains stationary
        if (RandomMovementPath != 0)
        {
            float Offset = Mathf.PingPong(Time.time * Speed, MovementDist * 2) - MovementDist;

            switch (RandomMovementPath)
            {
                case 1: // Vertical movement
                    transform.position = InitPos + new Vector3(0, Offset, 0);
                    break;

                case 2: // Horizontal movement
                    transform.position = InitPos + new Vector3(Offset, 0, 0);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.currentScore += AdditionalPoints * 10;

            Debug.Log($"+{AdditionalPoints} BONUS!");
            SoundManager.Play("PowerUp");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
