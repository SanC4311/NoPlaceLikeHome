using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public AudioManager audioManager;
    public void PlayGame()
    {
        audioManager.StopMenuAmbience();
        audioManager.StopMenuOpeningSound();
        audioManager.StartGameAmbience();
        audioManager.PlayGameOpeningSound();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void setResolutionTo3840x2160()
    {
        Screen.SetResolution(3840, 2160, true);
    }

    public void setResolutionTo2560x1440()
    {
        Screen.SetResolution(2560, 1440, true);
    }

    public void setResolutionTo1920x1080()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    public void setResolutionTo1280x720()
    {
        Screen.SetResolution(1280, 720, true);
    }
}
