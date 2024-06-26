using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmo : PlayerControl
{

    private int maxInteractDistance = 1;

    [SerializeField] AudioHandler audioHandler;

    public override List<GridPosition> GetValidPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition playerGridPosition = playerChar.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = playerGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                AmmoPile ammoPile = LevelGrid.Instance.GetAmmoPileAtGridPosition(testGridPosition);

                if (ammoPile == null)
                {
                    // No Defense on this GridPosition
                    continue;
                }

                //validGridPositionList.Add(testGridPosition);
                DoControl(testGridPosition, returningNull);
            }
        }

        return validGridPositionList;
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    private void returningNull()
    {
        return;
    }

    public override void DoControl(GridPosition gridPosition, Action onActionComplete)
    {
        AmmoPile ammoPile = LevelGrid.Instance.GetAmmoPileAtGridPosition(gridPosition);

        if (ammoPile != null)
        {
            audioHandler.PlayReload();
            playerChar.reloadBullets();
            ammoPile.Reload(OnReloadComplete);
        }

        ActionStart(onActionComplete);
    }

    private void OnReloadComplete()
    {
        playerChar.OnReloadComplete();
        ActionComplete();
        StartCoroutine(StopReloadSound());
    }

    private IEnumerator StopReloadSound()
    {
        yield return new WaitForSeconds(2f);
        audioHandler.StopReload();
    }
}
