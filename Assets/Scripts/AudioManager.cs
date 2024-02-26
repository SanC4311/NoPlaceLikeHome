using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] ambienceSounds;
    public AudioSource gameAmbienceSource;
    public AudioSource menuAmbienceSource;
    public AudioSource gameOpeningSource;
    public AudioSource menuOpeningSource;

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
        StartCoroutine(PlayMenuAmbience());
        PlayMenuOpeningSound();
    }

    private IEnumerator PlayMenuAmbience()
    {
        menuAmbienceSource.volume = 0;
        menuAmbienceSource.clip = ambienceSounds[0].soundClip;
        menuAmbienceSource.Play();

        while (menuAmbienceSource.volume < 0.4)
        {
            // Do it slower
            menuAmbienceSource.volume += Time.deltaTime / 20;
            yield return null;
        }
    }

    public void StopMenuAmbience()
    {
        menuAmbienceSource.Stop();
    }

    public void StopMenuOpeningSound()
    {
        menuOpeningSource.Stop();
    }

    public void PlayMenuOpeningSound()
    {
        StartCoroutine(PlayMenuOpening());
    }

    private IEnumerator PlayMenuOpening()
    {
        yield return new WaitForSeconds(1.5f);
        menuOpeningSource.Play();

        while (menuOpeningSource.volume < 0.6)
        {
            // Do it slower
            menuOpeningSource.volume += Time.deltaTime / 2;
            yield return null;
        }
    }

    public void StartGameAmbience()
    {
        StartCoroutine(PlayGameAmbience());
    }

    // Fade in play ambience 

    public IEnumerator PlayGameAmbience()
    {
        gameAmbienceSource.volume = 0;
        gameAmbienceSource.clip = ambienceSounds[1].soundClip;
        gameAmbienceSource.Play();

        while (gameAmbienceSource.volume < 0.4)
        {
            // Do it slower
            gameAmbienceSource.volume += Time.deltaTime / 20;
            yield return null;
        }
    }

    public void PlayGameOpeningSound()
    {
        StartCoroutine(PlayGameOpening());
    }

    private IEnumerator PlayGameOpening()
    {
        yield return new WaitForSeconds(1.5f);
        gameOpeningSource.Play();

        while (gameOpeningSource.volume < 0.6)
        {
            // Do it slower
            gameOpeningSource.volume += Time.deltaTime / 2;
            yield return null;
        }
    }

}
