using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;

public class GameManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private PrefabSpawner prefabSpawner;
    [SerializeField] private PlayerMove player;
    [SerializeField] private NoFlyZoneArea noFlyZone;

    [Header("Player Components")]
    public Transform playerTransform;
    public MeshRenderer playerRenderer;

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
    internal int currentScore;                               // track current score during run time

    [Header("Gameplay")]
    public int initialLives = 1;                            // Initial player life count
    public int Lives;                                       // Current player lives
    public float restartButtonDelay = 1.5f;
    public GameStates currentState = GameStates.PAUSED;

    [Header("Particles")]
    public GameObject explosion;

    [Header("UI")]
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button RestartBtn;
    [SerializeField] private TextMeshProUGUI CurrentScore_UI;
    [SerializeField] private TextMeshProUGUI HighScore_UI;
    [SerializeField] private TextMeshProUGUI CurrentSpeed_UI;
    [Space]
    [SerializeField] private Image zoneTimer;
    [SerializeField] private TextMeshProUGUI Warning_UI;
    [SerializeField] private Slider SliderIPU;
    [SerializeField] private Slider SliderSPU;

    private Coroutine scalingRoutine;
    private Coroutine warningRoutine;
    private bool exploded = false;

    private Coroutine invSliderRoutine;
    private Coroutine speedSliderRoutine;

    void Start()

    {
        // get script references
        saveManager = FindFirstObjectByType<SaveManager>();
        prefabSpawner = FindFirstObjectByType<PrefabSpawner>();
        player = FindFirstObjectByType<PlayerMove>();
        noFlyZone = FindFirstObjectByType<NoFlyZoneArea>();

        // player component references
        playerTransform = GameObject.FindWithTag("Player").transform;
        playerRenderer = GameObject.FindWithTag("Player").GetComponent<MeshRenderer>();

        zoneTimer.fillAmount = 0f;
        RestartBtn.gameObject.SetActive(false);

        UpdateUI();
        SetGameState(GameStates.PAUSED);
        SoundManager.PlayLoop("StartMusic");
    }

    void Update()
    {
        currentSpeed = player.forwardForce;
        CurrentSpeed_UI.text = $"<b>Current Speed: </b>{(currentSpeed / 100f):F0} km/h";

        HandleGameState();
    }

    // decided to use switch case using enums for more clarity
    private void HandleGameState()
    {
        switch (currentState)
        {
            case GameStates.RUNNING:
                exploded = false;
                player.canMove = true;
                playerRenderer.enabled = true;
                player.gameObject.SetActive(true);

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

                if (!exploded)
                {
                    exploded = true;
                    Instantiate(explosion, playerTransform.position, Quaternion.identity);
                    player.gameObject.SetActive(false);
                    SoundManager.Play("Plane_Crash");

                    StartCoroutine(ShowRestartButtonDelayed());
                }

                player.canMove = false;
                prefabSpawner.enabled = false;

                //if (!RestartBtn.gameObject.activeSelf) RestartBtn.gameObject.SetActive(true); //messes with restart delay

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

        CurrentScore_UI.text = $"<b>Score: </b>{currentScore/10}";
        HighScore_UI.text = $"<b>High Score: </b>{highScore/10}";

        currentScore++;
    }

    // start game with initial settings
    public void StartGame()
    {
        SoundManager.Play("Click");
        currentScore = 0;
        Lives = initialLives;
        player.transform.position = player.InitialPosition;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Run");

        // removes any momentum stored
        player.rb.linearVelocity = Vector3.zero;
        player.rb.angularVelocity = Vector3.zero;

        // cap initial speed
        player.forwardForce = Mathf.Min(initialSpeed, 6000);

        SetGameState(GameStates.RUNNING);
        SoundManager.PlayLoop("FlyingMusic");
    }

    // restart game by reloading scene to fully refresh everything
    public void RestartGame()
    {
        StartCoroutine(RestartRoutine());
    }

    // difficulty scaling over time
    private IEnumerator SpeedScalingDifficulty()
    {
        while (currentState == GameStates.RUNNING)
        {
            yield return new WaitForSeconds(timeToScale);
            player.forwardForce += speedIncrease;

            // cap only if not boosted
            if (!player.isBoosted && player.forwardForce > player.maxForwardSpeed)
                player.forwardForce = player.maxForwardSpeed;
        }
    }

    // function name says it all
    private void UpdateUI()
    {
        CurrentScore_UI.text = $"<b>Score: </b>{currentScore / 10}";
        HighScore_UI.text = $"<b>High Score: </b>{saveManager.saveData.HighScore / 10}";
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
            SoundManager.Play("Immunity");
            invSliderRoutine = StartCoroutine(PowerUpTimer(SliderIPU, duration));
        }
        else if (type == "Speed")
        {
            if (speedSliderRoutine != null) StopCoroutine(speedSliderRoutine);
            SoundManager.Play("Speed");
            speedSliderRoutine = StartCoroutine(PowerUpTimer(SliderSPU, duration));
        }
    }

    private IEnumerator ShowRestartButtonDelayed()
    {
        //Wait for the specified amount of seconds
        yield return new WaitForSeconds(restartButtonDelay);

        //Show the button
        if (RestartBtn != null)
        {
            RestartBtn.gameObject.SetActive(true);
        }
    }

    private IEnumerator RestartRoutine()
    {
        SoundManager.Play("Click");
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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