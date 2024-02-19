using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseHealth : MonoBehaviour
{

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
            Destroy(gameObject); // trigger a "destroyed" animation
        }
    }
}
