using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseHealth : MonoBehaviour
{

    public GameObject gameOverScreen;
    public GameObject gameActive;
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public class OnHealthChangedEventArgs : EventArgs
    {
        public float healthNormalised;
    }

    public float maxHealth = 100f;

    public float health = 100f;

    public bool targetDestroyed = false;

    public void TakeDamage(float amount)
    {

        health -= amount;
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            healthNormalised = health / maxHealth
        });

        if (health <= 0f)
        {
            OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
            {
                healthNormalised = 0f
            });
            targetDestroyed = true;
            // Show the game over screen
            gameOverScreen.SetActive(true);
            gameActive.SetActive(false);
            // Optionally, stop all game actions or pause the game
            Time.timeScale = 0; // Stops the game time, effectively pausing the game
        }
    }
}
