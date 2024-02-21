using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] ambienceSounds;
    public AudioSource ambienceSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayAmbience("Ambience");
    }

    public void PlayAmbience(string name)
    {
        Sound s = Array.Find(ambienceSounds, x => x.soundName == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            ambienceSource.clip = s.soundClip;
            ambienceSource.Play();
        }
    }


}
