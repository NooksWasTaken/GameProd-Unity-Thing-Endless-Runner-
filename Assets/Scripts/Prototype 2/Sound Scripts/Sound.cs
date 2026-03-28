using UnityEngine;

[System.Serializable]
public class Sound
{
    [Header("Sound Config")]
    public string name;
    public AudioClip clip;
    public SoundCategory category;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    public bool loop;
}