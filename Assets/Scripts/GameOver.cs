using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI overTimer;
    public Timer gameTimer;
    public AudioSource gameOverSound;

    public void TryAgain()
    {
        gameOverSound.Stop();
        Time.timeScale = 1; // Resumes the game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene
    }

    public void QuitGame()
    {
        Application.Quit(); // Quits the application
    }

    public void Start()
    {
        overTimer.text = gameTimer.showTime();
        gameOverSound.Play();
    }

}
