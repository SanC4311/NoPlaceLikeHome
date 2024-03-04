using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameTutorial : MonoBehaviour
{
    public GameObject GameActive;
    public GameObject[] TutorialScreens;
    public GameObject[] Players;
    public bool TutorialOver = false;

    public bool TutorialSkipped = false;


    void Start()
    {
        GameActive.SetActive(false);
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].SetActive(false);
        }
        //Pause time
        Time.timeScale = 0;
    }

    public void SkipTutorial()
    {
        TutorialSkipped = true;
        for (int i = 0; i < TutorialScreens.Length; i++)
        {
            TutorialScreens[i].SetActive(false);
        }
        TutorialOver = true;
        StartCoroutine(StartGame());
    }

    public void LateUpdate()
    {
        if (!TutorialSkipped)
        {
            PlayTutorial();
        }
    }

    public void PlayTutorial()
    {
        //Input Manager for the tutorial
        if (GameInput.Instance.isLeftMouseButtonDownThisFrame() && !TutorialOver)
        {
            for (int i = 0; i < TutorialScreens.Length; i++)
            {
                if (TutorialScreens[i].activeSelf)
                {
                    TutorialScreens[i].SetActive(false);
                    if (i + 1 < TutorialScreens.Length)
                    {
                        if (!TutorialSkipped)
                        {
                            TutorialScreens[i + 1].SetActive(true);
                        }
                    }
                    else
                    {
                        TutorialOver = true;
                        StartCoroutine(StartGame());
                    }
                    break;
                }
            }
        }
    }

    private IEnumerator StartGame()
    {
        AudioManager.Instance.StartGameAmbience();
        AudioManager.Instance.PlayGameOpeningSound();
        yield return new WaitForSecondsRealtime(0.005f);
        GameActive.SetActive(true);
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].SetActive(true);
        }
        //Resume time
        Time.timeScale = 1;
    }

}
