using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Scripts")]
    SectionManager sectionSpawner;
    PlayerMovement player;

    [Header("Scoring")]
    int HighScore;              // I assume this is to be preserved and not reset when the you quit, we'll deal with the json part later on
    int CurrentScore;       

    [Header("Gameplay")]
    float TimeElapsed;          // Adding this just in case it's needed
    public float LevelSpeed;    // Movement/Scroll speed of the level; increments every 60s
    [Space]
    public int Lives;           // Initial player life count
    internal bool IsInGame;

    [Header("Gameplay")]
    [SerializeField] private Button StartBtn;
    [SerializeField] private TextMeshProUGUI CurrentScore_UI;
    [SerializeField] private TextMeshProUGUI HighScore_UI;

    void Start()
    {
        Lives = 1;
        IsInGame = false;

        sectionSpawner = FindFirstObjectByType<SectionManager>();
        player = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        GameState();
    }

    void GameState()
    {
        IsInGame = Lives > 0;

        if (IsInGame)
        {
            sectionSpawner.enabled = true;
            player.enabled = true;

            if (StartBtn.gameObject.activeSelf) StartBtn.gameObject.SetActive(false);

            ScoreCounter();
        }
        else
        {
            sectionSpawner.enabled = false;
            player.enabled = false;

            if (!StartBtn.gameObject.activeSelf) StartBtn.gameObject.SetActive(true);
        }
    }

    void ScoreCounter()
    {
        CurrentScore++;
        if (CurrentScore > HighScore) HighScore = CurrentScore;

        CurrentScore_UI.text = $"<b>Score: </b>{CurrentScore}";
        HighScore_UI.text = $"<b>High Score: </b>{HighScore}";
    }

    public void StartGame()
    {
        Lives = 1;
        CurrentScore = 0;

        player.transform.position = player.InitialPosition;

        IsInGame = true;
    }
}
