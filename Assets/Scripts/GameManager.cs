using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scoring")]
    int HighScore;              // I assume this is to be preserved and not reset when the you quit, we'll deal with the json part later on
    int CurrentScore;       

    [Header("Gameplay")]
    float TimeElapsed;          // Adding this just in case it's needed
    public float LevelSpeed;    // Movement/Scroll speed of the level; increments every 60s
    [Space]
    public int Lives;           // Initial player life count


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
