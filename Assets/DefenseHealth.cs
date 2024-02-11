using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseHealth : MonoBehaviour
{
    public float health = 100f;

    public bool targetDestroyed = false;

    public void TakeDamage(float amount)
    {

        health -= amount;
        if (health <= 0f)
        {
            targetDestroyed = true;
            Destroy(gameObject); // trigger a "destroyed" animation
        }
    }
}
