using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] ambienceSounds;
    public AudioSource ambienceSource;
    public AudioSource openingSource;

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
        PlayOpeningSound();
    }

    // Fade in play ambience 

    private IEnumerator PlayAmbience()
    {
        ambienceSource.volume = 0;
        ambienceSource.clip = ambienceSounds[0].soundClip;
        ambienceSource.Play();

        while (ambienceSource.volume < 0.4)
        {
            // Do it slower
            ambienceSource.volume += Time.deltaTime / 20;
            yield return null;
        }
    }

    public void PlayOpeningSound()
    {
        StartCoroutine(PlayOpening());
    }

    private IEnumerator PlayOpening()
    {
        yield return new WaitForSeconds(1.5f);
        openingSource.Play();

        while (openingSource.volume < 0.6)
        {
            // Do it slower
            openingSource.volume += Time.deltaTime / 2;
            yield return null;
        }
    }






}
