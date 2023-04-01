using System;
using UnityEngine;

// Credit to Brackeys youtube tutorial on Audio managers, as the majority of this code was made by him.

public class AudioManager : MonoBehaviour
{
    // Uses Sound class to make array to access/manipulate all sounds
    public Sound[] sounds;
    // Variable created to manage location-based themes
    public string currTheme = "MainMenuTheme";
    public static AudioManager am;

    void Awake()
    {
        // Null check
        if (am == null)
            am = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Keeps object alive through scene changes
        DontDestroyOnLoad(gameObject);

        // Applies changes from editor to each of the sound clips
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        // Begins by playing default current theme
        Play(currTheme);
    }

    // Plays the song named in the string parameter
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) 
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    // this addition to the code was made by the person I borrowed this from, and the rest was from Brackeys original tutorial
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Stop();
    }
}