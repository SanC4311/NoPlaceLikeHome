using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseHealthUI : MonoBehaviour
{
    [SerializeField] private DefenseHealth defenseHealth;
    [SerializeField] private Image healthBar;

    private GridPosition gridPosition;

    private Action onInteractComplete;
    private bool isActive;
    private float timer;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetDefenseAtGridPosition(gridPosition, this);
        defenseHealth.OnHealthChanged += DefenseHealth_OnHealthChanged;
        healthBar.fillAmount = 1f;
    }

    private void DefenseHealth_OnHealthChanged(object sender, DefenseHealth.OnHealthChangedEventArgs e)
    {
        healthBar.fillAmount = e.healthNormalised;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = false;
            onInteractComplete();
        }
    }

    public void Repair(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = .5f;

        defenseHealth.health = 100f;
        healthBar.fillAmount = 1f;
    }

}
