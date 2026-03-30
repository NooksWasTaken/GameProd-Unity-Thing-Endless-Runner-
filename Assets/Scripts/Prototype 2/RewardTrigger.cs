using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class RewardTrigger : MonoBehaviour
{
    GameManager gameManager;

    public int AdditionalPoints;
    int RandomMovementPath;
    public float MovementDist;
    public float Speed;

    Vector3 InitPos;

    TextMeshProUGUI Points_UI;

    void Start()
    {
        Points_UI = GameObject.Find("Bonus Point Text").GetComponent<TextMeshProUGUI>();

        // Fixes rotation
        transform.rotation = Quaternion.Euler(90, 0, 0);

        // Randomizes elevation
        int randElevation = Random.Range(-5, 6);
        transform.position += new Vector3(0, randElevation, 0);

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

            Color color = Points_UI.color;
            Points_UI.color = new Color(color.r, color.g, color.b, 1);

            StartCoroutine("HideMessage");
        }
    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(2);

        Color color = Points_UI.color;
        Points_UI.color = new Color(color.r, color.g, color.b, 0);
        Destroy(gameObject);
    }
}
