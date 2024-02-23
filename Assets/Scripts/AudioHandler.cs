using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource footsSource;
    [SerializeField] private AudioSource gunSource;
    [SerializeField] private AudioSource reloadSource;
    [SerializeField] private AudioSource repairSource;
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
        //Debug.Log("Gun sound: " + randomGunSound);
        gunSource.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
        gunSource.Play();
    }

    public void StopGunshot()
    {
        gunSource.Stop();
    }

    public void PlayReload()
    {
        reloadSource.Play();
    }

    public void StopReload()
    {
        reloadSource.Stop();
    }

    public void PlayRepair()
    {
        repairSource.Play();
    }

    public void StopRepair()
    {
        repairSource.Stop();
    }
    public void PlaySpawn()
    {
        reloadSource.pitch = UnityEngine.Random.Range(0.7f, 1.4f);
        reloadSource.Play();
    }
    public void PlayDeath()
    {
        repairSource.pitch = UnityEngine.Random.Range(0.7f, 1.4f);
        repairSource.Play();
    }


}
