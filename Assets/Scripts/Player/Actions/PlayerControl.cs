using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControl : MonoBehaviour
{
    protected PlayerChar playerChar;
    protected bool isActive;
    protected Action onActionComplete;

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;


    protected virtual void Awake()
    {
        playerChar = GetComponent<PlayerChar>();
    }

    public abstract void DoControl(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidPosition(GridPosition gridPosition)
    {
        List<GridPosition> validPositionList = GetValidPositionList();
        return validPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidPositionList();

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

}
