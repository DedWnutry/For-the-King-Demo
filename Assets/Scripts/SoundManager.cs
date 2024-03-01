using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    
    void Start()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;

            sound.source.pitch = sound.pitch;

            sound.source.loop = sound.loop;
        }
    }

    public void PlaySound(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        sound.source.Play();
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public bool loop;
    [Range(0f, 1f)] public float volume = .5f;
    [Range(.1f, 1f)] public float pitch = 1f;
    public AudioSource source;
    public SoundCategory category;
}

public enum SoundCategory { OST, Voice, Effect }
