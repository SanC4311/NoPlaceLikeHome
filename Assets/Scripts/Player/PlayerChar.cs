using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar : MonoBehaviour
{
    private GridPosition gridPosition;
    private PlayerMove playerMove;
    //private PlayerDefendRifle playerDefend;
    private PlayerControl[] playerControls;

    public bool attackMode = false;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        //playerDefend = GetComponent<PlayerDefendRifle>();
        playerControls = GetComponents<PlayerControl>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddPlayerCharAtGridPosition(gridPosition, this);
    }

    private void Update()
    {

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.PlayerCharMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }

        Debug.Log(attackMode);
        Debug.Log("Current position: " + gridPosition.x + ", " + gridPosition.z);
        AttackMode();
    }

    public PlayerMove GetPlayerMove()
    {
        return playerMove;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public PlayerControl[] GetPlayerControls()
    {
        return playerControls;
    }

    public void AttackMode()
    {
        float tolerance = 0.5f;
        if ((gridPosition.x >= 1 - tolerance && gridPosition.x <= 1 + tolerance) &&
            (gridPosition.z >= 4 - tolerance && gridPosition.z <= 4 + tolerance))
        {
            attackMode = true;
        }
        else
        {
            attackMode = false; // Optionally, disable attack mode if not within the target range
        }
    }

}
