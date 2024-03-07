using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public AudioManager audioManager;
    public GameObject buttons;

    public void Start()
    {
        StartCoroutine(ShowButtons());
    }

    private IEnumerator ShowButtons()
    {
        buttons.SetActive(false);
        yield return new WaitForSeconds(2f);
        buttons.SetActive(true);
    }
    public void PlayGame()
    {
        audioManager.StopMenuAmbience();
        audioManager.StopMenuOpeningSound();
        Loader.LoadScene(Loader.Scene.GameScene);
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
