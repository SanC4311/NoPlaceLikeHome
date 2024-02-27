using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorial : MonoBehaviour
{
    public GameObject GameActive;
    public GameObject[] TutorialScreens;
    public bool TutorialOver = false;

    void Start()
    {
        GameActive.SetActive(false);
        //Pause time
        Time.timeScale = 0;
    }

    public void Update()
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
                        TutorialScreens[i + 1].SetActive(true);
                    }
                    else
                    {
                        TutorialOver = true;
                        StartGame();
                    }
                    break;
                }
            }
        }
    }


    public void StartGame()
    {
        AudioManager.Instance.StartGameAmbience();
        AudioManager.Instance.PlayGameOpeningSound();
        GameActive.SetActive(true);
        //Resume time
        Time.timeScale = 1;
    }

}
