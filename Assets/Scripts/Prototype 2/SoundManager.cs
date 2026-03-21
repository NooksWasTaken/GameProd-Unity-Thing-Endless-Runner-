using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    // dawg im too lazy to comment every single step here im losing my sanity
    [Header("Audio Clips")]
    [SerializeField] private AudioClip bgmPause;
    [SerializeField] private AudioClip bgmRun;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource currentSource;
    private AudioSource nextSource;
    private GameStates lastState;

    void Awake()
    {
        currentSource = gameObject.AddComponent<AudioSource>();
        nextSource = gameObject.AddComponent<AudioSource>();

        currentSource.loop = true;
        nextSource.loop = true;

        currentSource.clip = bgmPause;
        currentSource.volume = 1f;
        currentSource.Play();

        lastState = GameStates.PAUSED;
    }

    void Update()
    {
        GameStates currentState = FindFirstObjectByType<GameManager>().currentState;

        if (currentState != lastState)
        {
            AudioClip nextClip = (currentState == GameStates.RUNNING) ? bgmRun : bgmPause;
            StartCoroutine(CrossfadeTo(nextClip));
            lastState = currentState;
        }
    }

    // this will create a smooth fade transition between the two bgms
    private IEnumerator CrossfadeTo(AudioClip newClip)
    {
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;

            currentSource.volume = Mathf.Lerp(1f, 0f, alpha);
            nextSource.volume = Mathf.Lerp(0f, 1f, alpha);

            yield return null;
        }

        currentSource.volume = 0f;
        nextSource.volume = 1f;

        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }

    // sound effects stuff go here augh
}