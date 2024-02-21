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
        StartCoroutine(PlayAmbience());
    }

    // Fade in play ambience 

    private IEnumerator PlayAmbience()
    {
        ambienceSource.volume = 0;
        ambienceSource.clip = ambienceSounds[0].soundClip;
        ambienceSource.Play();

        while (ambienceSource.volume < 0.6)
        {
            // Do it slower
            ambienceSource.volume += Time.deltaTime / 16;
            yield return null;
        }
    }




}
