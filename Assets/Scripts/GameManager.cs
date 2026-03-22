using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using GameAnalyticsSDK;

public class GameManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private PrefabSpawner prefabSpawner;
    [SerializeField] private PlayerMove player;
    [SerializeField] private NoFlyZoneArea noFlyZone;

    [Header("Out of Bounds Settings")]
    public float currentTime;
    public float maxTimeLimit;
    
    [Header("Difficulty Scaling Settings")]
    public int timeToScale;                                 // this value needs to be carefully adjusted, this is a linear scale and idk if we want exponential
    public int initialSpeed;
    public int speedIncrease;
    [Space]
    public float currentSpeed;                              // we'll only use this to track the speed of the player during run time, not for anyhting else

    [Header("Scoring")]
    private int highScore;                                  // high score now preserved via save manager yipeeeeee
    private int currentScore;                               // track current score during run time

    [Header("Gameplay")]
    public int initialLives = 1;                            // Initial player life count
    public int Lives;                                       // Current player lives
    public GameStates currentState = GameStates.PAUSED;

    [Header("UI")]
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button RestartBtn;
    [SerializeField] private TextMeshProUGUI CurrentScore_UI;
    [SerializeField] private TextMeshProUGUI HighScore_UI;
    [SerializeField] private TextMeshProUGUI CurrentSpeed_UI;
    [Space]
    [SerializeField] private Image zoneTimer;
    [SerializeField] private TextMeshProUGUI Warning_UI;

    private Coroutine scalingRoutine;
    private Coroutine warningRoutine;

    void Start()
    {
        // get references
        saveManager = FindFirstObjectByType<SaveManager>();
        prefabSpawner = FindFirstObjectByType<PrefabSpawner>();
        player = FindFirstObjectByType<PlayerMove>();
        noFlyZone = FindFirstObjectByType<NoFlyZoneArea>();

        zoneTimer.fillAmount = 0f;
        //Warning_UI.gameObject.SetActive(false);
        RestartBtn.gameObject.SetActive(false);

        UpdateUI();
        SetGameState(GameStates.PAUSED);
    }

    void Update()
    {
        currentSpeed = player.forwardForce;
        CurrentSpeed_UI.text = $"<b>Current Speed: </b>{currentSpeed}";

        HandleGameState();
    }

    // decided to use switch case using enums for more clarity
    private void HandleGameState()
    {
        switch (currentState)
        {
            case GameStates.RUNNING:
                player.canMove = true;

                if (StartBtn.gameObject.activeSelf) StartBtn.gameObject.SetActive(false);
                if (RestartBtn.gameObject.activeSelf) RestartBtn.gameObject.SetActive(false);

                ScoreCounter();

                // only start scaling once
                if (scalingRoutine == null)
                    scalingRoutine = StartCoroutine(SpeedScalingDifficulty());

                break;

            case GameStates.PAUSED:
                player.canMove = false;

                if (!StartBtn.gameObject.activeSelf) StartBtn.gameObject.SetActive(true);
                if (scalingRoutine != null)
                {
                    StopCoroutine(scalingRoutine);
                    scalingRoutine = null;
                }

                break;

            case GameStates.GAMEOVER:
                player.canMove = false;

                if (!RestartBtn.gameObject.activeSelf) RestartBtn.gameObject.SetActive(true);

                if (highScore > saveManager.saveData.HighScore)
                {
                    saveManager.saveData.HighScore = highScore;
                    saveManager.Save();
                }
         
                break;
        }
    }

    // score tracker
    private void ScoreCounter()
    {
        highScore = Mathf.Max(currentScore, saveManager.saveData.HighScore);

        CurrentScore_UI.text = $"<b>Score: </b>{currentScore}";
        HighScore_UI.text = $"<b>High Score: </b>{highScore}";

        currentScore++;
    }

    // start game with initial settings
    public void StartGame()
    {
        currentScore = 0;
        Lives = initialLives;
        player.transform.position = player.InitialPosition;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Run");

        // removes any momentum stored
        player.rb.linearVelocity = Vector3.zero;
        player.rb.angularVelocity = Vector3.zero;
        player.forwardForce = initialSpeed;

        SetGameState(GameStates.RUNNING);
    }

    // restart game by reloading scene to fully refresh everything
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // difficulty scaling over time
    private IEnumerator SpeedScalingDifficulty()
    {
        while (currentState == GameStates.RUNNING)
        {
            yield return new WaitForSeconds(timeToScale);
            player.forwardForce += speedIncrease;
        }
    }

    // function name says it all
    private void UpdateUI()
    {
        CurrentScore_UI.text = $"<b>Score: </b>{currentScore}";
        HighScore_UI.text = $"<b>High Score: </b>{saveManager.saveData.HighScore}";
    }

    // function name is obvious
    public void SetGameState(GameStates newState)
    {
        currentState = newState;
    }

    // start a timer if the player is out of bounds
    public void NoFlyZoneTimer()
    {
        if (warningRoutine == null)
        {
            warningRoutine = StartCoroutine(BlinkWarning());
        }

        currentTime = Mathf.Clamp(currentTime, 0f, maxTimeLimit);
        currentTime += Time.deltaTime;

        zoneTimer.fillAmount += 1f / maxTimeLimit * Time.deltaTime;

        if (currentTime >= maxTimeLimit)
        {
            SetGameState(GameStates.GAMEOVER);
        }
    }

    public void OnFlyZoneExit()
    {
        StartCoroutine(FlyZoneTimerDecrease());
    }

    // decrease the timer gradually if the player is out of bounds
    private IEnumerator FlyZoneTimerDecrease()
    {
        while (!noFlyZone.isPlayerInside)
        {
            currentTime = Mathf.Clamp(currentTime, 0f, maxTimeLimit);
            currentTime -= Time.deltaTime;

            zoneTimer.fillAmount -= 1f / maxTimeLimit * Time.deltaTime;

            if (currentTime <= 0f)
            {
                if (warningRoutine != null)
                {
                    StopCoroutine(warningRoutine);
                    warningRoutine = null;
                }

                Warning_UI.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }

    // flashing warning text
    private IEnumerator BlinkWarning()
    {
        while (currentTime > 0f)
        {
            Warning_UI.gameObject.SetActive(!Warning_UI.gameObject.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }

        Warning_UI.gameObject.SetActive(false);
        warningRoutine = null;
    }
}