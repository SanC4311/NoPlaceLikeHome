using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource footsSource;
    [SerializeField] private AudioSource gunSource;
    public AudioClip[] gunSounds;
    private int randomGunSound;

    public void PlayFootsteps(float pitchLow, float pitchHigh)
    {
        footsSource.pitch = UnityEngine.Random.Range(pitchLow, pitchHigh);
        footsSource.Play();
    }

    public void StopFootsteps()
    {
        footsSource.Pause();
    }

    public void PlayGunshot()
    {
        randomGunSound = Random.Range(0, 3);
        gunSource.clip = gunSounds[randomGunSound];
        Debug.Log("Gun sound: " + randomGunSound);
        gunSource.Play();
    }

    public void StopGunshot()
    {
        gunSource.Stop();
    }


}
