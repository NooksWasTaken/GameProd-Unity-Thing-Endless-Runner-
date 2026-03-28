using TMPro;
using UnityEngine;
using UnityEngine.UI;
<<<<<<< Updated upstream
=======
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using GameAnalyticsSDK;
using UnityEngine.Rendering;
>>>>>>> Stashed changes

public class GameManager : MonoBehaviour
{
    [Header("Scripts")]
    SectionManager sectionSpawner;
    PlayerMovement player;

    [Header("Scoring")]
    int HighScore;              // I assume this is to be preserved and not reset when the you quit, we'll deal with the json part later on
    int CurrentScore;       

    [Header("Gameplay")]
<<<<<<< Updated upstream
    float TimeElapsed;          // Adding this just in case it's needed
    public float LevelSpeed;    // Movement/Scroll speed of the level; increments every 60s
    [Space]
    public int Lives;           // Initial player life count
    internal bool IsInGame;
=======
    public int initialLives = 1;                            // Initial player life count
    public int Lives;                                       // Current player lives
    public float restartButtonDelay = 2f;
    public GameStates currentState = GameStates.PAUSED;
>>>>>>> Stashed changes

    [Header("Gameplay")]
    [SerializeField] private Button StartBtn;
    [SerializeField] private TextMeshProUGUI CurrentScore_UI;
    [SerializeField] private TextMeshProUGUI HighScore_UI;
<<<<<<< Updated upstream
=======
    [SerializeField] private TextMeshProUGUI CurrentSpeed_UI;
    [Space]
    [SerializeField] private Image zoneTimer;
    [SerializeField] private TextMeshProUGUI Warning_UI;
    [SerializeField] private Slider SliderIPU;
    [SerializeField] private Slider SliderSPU;

    [Header("Sliders")]
    [SerializeField] private Slider sliderIPU;
    [SerializeField] private Slider sliderSPU;
    public float startIpu = 4f;
    public float startSpu = 3f;
    private float current;
    private bool isCounting = false;


    private Coroutine scalingRoutine;
    private Coroutine warningRoutine;
>>>>>>> Stashed changes

    private Coroutine invSliderRoutine;
    private Coroutine speedSliderRoutine;

    void Start()
    {
        Lives = 1;
        IsInGame = false;

<<<<<<< Updated upstream
        sectionSpawner = FindFirstObjectByType<SectionManager>();
        player = FindFirstObjectByType<PlayerMovement>();
=======
        zoneTimer.fillAmount = 0f;
        //Warning_UI.gameObject.SetActive(false);
        RestartBtn.gameObject.SetActive(false);

        current = start;


        UpdateUI();
        SetGameState(GameStates.PAUSED);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            sectionSpawner.enabled = true;
            player.enabled = true;
=======
            case GameStates.RUNNING:
                exploded = false;
                player.canMove = true;
                playerRenderer.enabled = true;
                player.gameObject.SetActive(true);
>>>>>>> Stashed changes

            if (StartBtn.gameObject.activeSelf) StartBtn.gameObject.SetActive(false);

            ScoreCounter();
        }
        else
        {
            sectionSpawner.enabled = false;
            player.enabled = false;

<<<<<<< Updated upstream
            if (!StartBtn.gameObject.activeSelf) StartBtn.gameObject.SetActive(true);
=======
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

                if (!exploded)
                {
                    exploded = true;
                    Instantiate(explosion, playerTransform.position, Quaternion.identity);
                    //playerRenderer.enabled = false;
                    player.gameObject.SetActive(false);
                    SoundManager.Play("Plane_Crash");

                    StartCoroutine(ShowRestartButtonDelayed());
                }

                player.canMove = false;
                prefabSpawner.enabled = false;
                //Commented out messes with coroutine
                //if (!RestartBtn.gameObject.activeSelf) RestartBtn.gameObject.SetActive(true);
                
                if (highScore > saveManager.saveData.HighScore)
                {
                    saveManager.saveData.HighScore = highScore;
                    saveManager.Save();
                }

                break;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
}
=======

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

    public void StartPowerUpUI(string type, float duration)
    {
        if (type == "Invincibility")
        {
            if (invSliderRoutine != null) StopCoroutine(invSliderRoutine);
            invSliderRoutine = StartCoroutine(PowerUpTimer(SliderIPU, duration));
        }
        else if (type == "Speed")
        {
            if (speedSliderRoutine != null) StopCoroutine(speedSliderRoutine);
            speedSliderRoutine = StartCoroutine(PowerUpTimer(SliderSPU, duration));
        }
    }

    private IEnumerator PowerUpTimer(Slider slider, float duration)
    {
        slider.gameObject.SetActive(true);
        slider.maxValue = duration;
        float timer = duration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            slider.value = timer;
            yield return null;
        }

        slider.gameObject.SetActive(false);
    }

    public void OnFlyZoneExit()
    {
        StartCoroutine(FlyZoneTimerDecrease());
    }

    public void PowerUpTimer()
    {
        StartCoroutine(puTimerDecrease);
    }

    private IEnumerator puTimerDecrease()
    {
        
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
<<<<<<< Updated upstream
}
>>>>>>> Stashed changes
=======

    private IEnumerator ShowRestartButtonDelayed()
    {
        // Wait for the specified amount of seconds
        yield return new WaitForSeconds(restartButtonDelay);

        // Show the button
        if (RestartBtn != null)
        {
            RestartBtn.gameObject.SetActive(true);
        }
    }
}
>>>>>>> Stashed changes
