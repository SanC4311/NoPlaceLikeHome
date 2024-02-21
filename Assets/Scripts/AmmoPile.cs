using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPile : MonoBehaviour
{

    private GridPosition gridPosition;

    private Action onInteractComplete;
    private bool isActive;
    private float timer;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetAmmoPileAtGridPosition(gridPosition, this);
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

    public void Reload(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = .5f;
    }

}
