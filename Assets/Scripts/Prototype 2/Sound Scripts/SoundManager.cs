using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // this be static so it can be called across all scripts without much referencing
    private static SoundManager instance;

    // compilation of sounds, drag em here
    public Sound[] sounds;

    // uses sound.cs naming scheme, assign name to sound clip in inspector
    private Dictionary<string, Sound> soundData;

    // mixer groups
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;


    private AudioSource musicSource;
    private List<AudioSource> sfxSources;

    public int sfxPoolSize = 10;
    private int sfxIndex = 0;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        soundData = new Dictionary<string, Sound>();

        foreach (Sound s in sounds)
        {
            soundData.Add(s.name, s);
        }

        // audio source for music
        GameObject musicGO = new GameObject("MusicSource");
        musicGO.transform.parent = transform;
        musicSource = musicGO.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;

        // audio source for sfx
        sfxSources = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxGO = new GameObject("SFX_" + i);
            sfxGO.transform.parent = transform;
            AudioSource source = sfxGO.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxGroup;
            sfxSources.Add(source);
        }
    }

    // call these methods below depending on use case
    public static void Play(string name)
    {
        if (!instance.soundData.ContainsKey(name)) return;

        Sound s = instance.soundData[name];

        if (s.category == SoundCategory.Music)
        {
            instance.PlayMusic(s);
        }
        else
        {
            instance.PlaySFX(s);
        }
    }

    public static void PlayLoop(string name)
    {
        if (!instance.soundData.ContainsKey(name)) return;

        Sound s = instance.soundData[name];
        s.loop = true;

        Play(name);
    }

    public static void FadeIn(string name, float duration)
    {
        if (!instance.soundData.ContainsKey(name)) return;

        instance.StartCoroutine(instance.FadeInRoutine(name, duration));
    }

    public static void FadeOut(string name, float duration)
    {
        if (!instance.soundData.ContainsKey(name)) return;

        instance.StartCoroutine(instance.FadeOutRoutine(name, duration));
    }

    // don't call these functions belw

    private void PlayMusic(Sound s)
    {
        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.loop = true;

        musicSource.Play();
    }

    private void PlaySFX(Sound s)
    {
        AudioSource source = sfxSources[sfxIndex];
        sfxIndex = (sfxIndex + 1) % sfxSources.Count;

        source.clip = s.clip;
        source.volume = s.volume;
        source.pitch = s.pitch;
        source.loop = false;

        source.Play();
    }

    public static void Stop(string name)
    {
        if (!instance.soundData.ContainsKey(name)) return;

        Sound s = instance.soundData[name];

        if (s.category == SoundCategory.Music)
        {
            instance.musicSource.Stop();
        }
        else
        {
            instance.StopSFX(s);
        }
    }
    private void StopSFX(Sound s)
    {
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying && source.clip == s.clip)
            {
                source.Stop();
            }
        }
    }

    private IEnumerator FadeInRoutine(string name, float duration)
    {
        Sound s = soundData[name];

        if (s.category != SoundCategory.Music) yield break;

        musicSource.clip = s.clip;
        musicSource.volume = 0f;
        musicSource.Play();

        float target = s.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, target, time / duration);
            yield return null;
        }

        musicSource.volume = target;
    }

    private IEnumerator FadeOutRoutine(string name, float duration)
    {
        Sound s = soundData[name];

        if (s.category != SoundCategory.Music) yield break;

        float start = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(start, 0f, time / duration);
            yield return null;
        }

        musicSource.Stop();
    }
}