using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseHealthUI : MonoBehaviour
{
    [SerializeField] private DefenseHealth defenseHealth;
    [SerializeField] private Image healthBar;

    private void Start()
    {
        defenseHealth.OnHealthChanged += DefenseHealth_OnHealthChanged;
        healthBar.fillAmount = 1f;
    }

    private void DefenseHealth_OnHealthChanged(object sender, DefenseHealth.OnHealthChangedEventArgs e)
    {
        healthBar.fillAmount = e.healthNormalised;
    }

}
